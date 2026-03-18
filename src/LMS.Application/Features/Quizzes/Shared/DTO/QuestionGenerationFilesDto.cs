namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class QuestionGenerationFilesDto
{
    public Guid? SectionId { get; set; }
    public string? SectionName { get; set; }
    public List<FileDto> Files { get; set; }
}

public class FileDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
}
