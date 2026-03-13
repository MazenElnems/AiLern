using Hangfire;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using LMS.Infrastructure.ExternalServices.AIService.Contracts;
using LMS.Infrastructure.ExternalServices.AIService.Models;
using LMS.Infrastructure.ExternalServices.AIService.Requests;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace LMS.Infrastructure.Jobs;

public class GenerateQuestionsJob : IGenerateQuestionsJob
{
    private readonly IAIService _service;
    private readonly IWasabiService _wasabiService;
    private readonly IUnitOfWork _unitOfWork;

    public GenerateQuestionsJob(IAIService service, IWasabiService wasabiService, IUnitOfWork unitOfWork)
    {
        _service = service;
        _wasabiService = wasabiService;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid jobId,
        Guid quizId,
        List<string> materialFileIds,
        List<IFormFile> files,
        int questionsCount,
        Dictionary<QuestionType, int> questionTypeCounts,
        Dictionary<QuestionDifficultyLevels, float> questionDifficultyPercents,
        CancellationToken token,
        string? query = null)
    {
        var openedStreams = new List<Stream>();
        try
        {
            var job = await _unitOfWork.QuestionGenerationJobs.GetByIdAsync(jobId);
            if(job == null)
                return;

            job.Status = AIJobStatus.InProgress;
            job.Error = null;
            job.CompletedAt = null;
            await _unitOfWork.CommitAsync();

            var ids = materialFileIds.Select(Guid.Parse).ToList();
            var materialFiles = (await _unitOfWork.MaterialFiles
                .FilterAsync(m => !m.HasUploadedToAIService && ids.Contains(m.Id)))
                .ToList();

            token.ThrowIfCancellationRequested();
            var materialFileStreams = await _wasabiService.GetFileStreamAsync(
                materialFiles.Select(m => m.StoragePath).ToList());
            openedStreams.AddRange(materialFileStreams);


            var uploadedFileStreams = files.Select(f => f.OpenReadStream()).ToList();
            openedStreams.AddRange(uploadedFileStreams);

            var fileNames = materialFiles.Select(m => m.FileName)
                .Concat(files.Select(f => f.FileName))
                .ToList();

            var uploadedNewFileProjectIds = files.Select(_ => Guid.NewGuid().ToString()).ToList();
            var projectIds = materialFiles.Select(m => m.Id.ToString())
                .Concat(uploadedNewFileProjectIds)
                .ToList();

            var streams = materialFileStreams
                .Concat(uploadedFileStreams)
                .ToList();

            var responses = new List<AIUploadFilesResponse>();
            for(var i = 0; i < projectIds.Count; i++)
            {
                token.ThrowIfCancellationRequested();

                var response = await _service.UploadFileAsync(projectIds[i], fileNames[i], streams[i]);
                responses.Add(response);
            }

            var failedUpload = responses.FirstOrDefault(r => !r.Status.Equals("ok", StringComparison.OrdinalIgnoreCase));
            if(failedUpload != null)
            {
                job.Status = AIJobStatus.Failed;
                job.Error = $"Failed to upload file {failedUpload.Filename} to AI service.";
                job.CompletedAt = DateTime.UtcNow;
                await _unitOfWork.CommitAsync();
                return;
            }

            foreach(var materialFile in materialFiles)
            {

                materialFile.HasUploadedToAIService = true;
                _unitOfWork.MaterialFiles.Update(materialFile);
            }

            // Reuse already uploaded AI project ids for material files that were not re-uploaded.
            var allProjectIds = materialFiles.Select(m => m.Id.ToString())
                .Concat(uploadedNewFileProjectIds)
                .Concat(materialFileIds)
                .Distinct()
                .ToArray();

            var request = new AIQuizGenerationRequest
            {
                ProjectIDs = allProjectIds,
                NumberOfQuestions = questionsCount,
                QuestionTypeCount = questionTypeCounts,
                QuestionDifficultyPercents = questionDifficultyPercents,
                Query = query
            };


            token.ThrowIfCancellationRequested();
            var result = await _service.GenerateQuestionsAsync(request);

            if(!result.Status.Equals("ok", StringComparison.OrdinalIgnoreCase))
            {
                job.Status = AIJobStatus.Failed;
                job.Error = $"Failed to generate questions. Message: {result.Message}";
                job.CompletedAt = DateTime.UtcNow;
                await _unitOfWork.CommitAsync();
                return;
            }

            var existingCount = await _unitOfWork.Questions.CountAsync(q => q.QuizId == quizId);
            var order = existingCount + 1;
            foreach(var generatedQuestion in result.Questions ?? [])
            {
                token.ThrowIfCancellationRequested();

                var question = MapQuestion(generatedQuestion, quizId, order++);
                await _unitOfWork.Questions.InsertAsync(question);
            }


            job.Status = AIJobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            job.Error = null;

            await _unitOfWork.CommitAsync();
        }
        catch (OperationCanceledException)
        {
            var job = await _unitOfWork.QuestionGenerationJobs.GetByIdAsync(jobId);
            if (job != null)
            {
                job.Status = AIJobStatus.Canceled;
                job.Error = "Job Canceled by User";
                job.CompletedAt = DateTime.UtcNow;
                await _unitOfWork.CommitAsync();
            }
        }
        catch (Exception ex)
        {
            var job = await _unitOfWork.QuestionGenerationJobs.GetByIdAsync(jobId);
            if(job != null)
            {
                job.Status = AIJobStatus.Failed;
                job.Error = ex.Message;
                job.CompletedAt = DateTime.UtcNow;
                await _unitOfWork.CommitAsync();
            }
        }
        finally
        {
            foreach(var stream in openedStreams)
                stream.Dispose();
        }
    }

    private static Question MapQuestion(AIQuestionGeneratedResponse generatedQuestion, Guid quizId, int order)
    {
        var question = new Question
        {
            Id = Guid.NewGuid(),
            QuizId = quizId,
            QuestionText = generatedQuestion.Question,
            Type = generatedQuestion.QuestionType,
            Mark = 1,
            Order = order,
            Instructions = null,
            Explanation = generatedQuestion.Answer ?? generatedQuestion.Explaination
        };

        if(generatedQuestion.QuestionType != QuestionType.Written)
        {
            var correctAnswer = generatedQuestion.CorrectAnswer?.Trim() ?? string.Empty;
            var options = generatedQuestion.Options ?? [];
            var optionNumber = 1;

            foreach(var optionText in options)
            {
                var normalizedOption = optionText?.Trim() ?? string.Empty;
                question.Options.Add(new Option
                {
                    OptionNumber = optionNumber++,
                    OptionText = optionText,
                    IsCorrect = string.Equals(normalizedOption, correctAnswer, StringComparison.OrdinalIgnoreCase),
                    QuestionId = question.Id
                });
            }

            if(question.Options.Count > 0 && question.Options.All(o => !o.IsCorrect))
            {
                if(int.TryParse(correctAnswer, NumberStyles.Integer, CultureInfo.InvariantCulture, out var correctIndex)
                   && correctIndex >= 1
                   && correctIndex <= question.Options.Count)
                {
                    question.Options[correctIndex - 1].IsCorrect = true;
                }
            }
        }

        return question;
    }
}
