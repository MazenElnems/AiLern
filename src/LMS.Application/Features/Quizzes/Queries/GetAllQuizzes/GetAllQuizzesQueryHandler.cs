using AutoMapper;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace LMS.Application.Features.Quizzes.Queries.GetAllQuizzes
{
    public class GetAllQuizzesQueryHandler : IRequestHandler<GetAllQuizzesByCourseIdQuery, Result<PaginationResult<GetAllQuizDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllQuizzesQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetAllQuizzesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllQuizzesQueryHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<PaginationResult<GetAllQuizDto>>> Handle(GetAllQuizzesByCourseIdQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber < 1 || request.PageSize < 1)
                return Result<PaginationResult<GetAllQuizDto>>.Failure(DomainErrors.Pagination.InvalidParameters);

            try
            {
                Expression<Func<Quiz, bool>> predicate = c => true;

                var sortBy = request.SortBy;
                var order = request.Order?.ToLower();
                var isDescending = order != SortOrderOptions.ASC;

                Expression<Func<Quiz, object>> orderBy = sortBy?.ToLower() switch
                {
                    var s when s == QuizSortByOptions.Title => c => c.Title,
                    var s when s == QuizSortByOptions.CreatedAt => c => c.CreatedAt,
                    var s when s == QuizSortByOptions.MaximumAttempts => c => c.MaximumAttempts,
                    var s when s == QuizSortByOptions.TotalPoints => c => c.TotalPoints,
                    _ => c => c.CreatedAt
                };

                var totalResult = await _unitOfWork.Quizzes.CountAsync(predicate);

                if (totalResult == 0)
                {
                    var emptyResult = new PaginationResult<GetAllQuizDto>(request.PageNumber, request.PageSize, 0, new List<GetAllQuizDto>());
                    return Result<PaginationResult<GetAllQuizDto>>.Success(emptyResult);
                }

                var quizzes = await _unitOfWork.Quizzes.FilterAsync(
                    predicate,
                    orderBy,
                    isDescending,
                    (request.PageNumber - 1) * request.PageSize,
                    request.PageSize);

                var dto = _mapper.Map<List<GetAllQuizDto>>(quizzes);
                return Result<PaginationResult<GetAllQuizDto>>.Success(
                    new PaginationResult<GetAllQuizDto>(request.PageNumber, request.PageSize, totalResult, dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving courses.");
                throw;
            }

        }
    }
}
