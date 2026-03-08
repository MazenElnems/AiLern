using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LMS.Application.Features.Quizzes.Queries.GetQuiz;

public class GetQuizByIdQuery : IRequest<Result<GetQuizDto>>
{
    [BindNever]
    public Guid Id { get; set; }
}
