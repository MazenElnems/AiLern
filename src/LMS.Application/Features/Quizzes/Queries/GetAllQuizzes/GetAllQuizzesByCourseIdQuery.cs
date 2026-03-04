using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Courses.DTO;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.Features.Quizzes.Queries.GetAllQuizzes
{
    public class GetAllQuizzesByCourseIdQuery : BasePaginatedQuery , IRequest<Result<PaginationResult<GetAllQuizDto>>>
    {
        [BindNever]
        public int CourseId { get; set; } 
    }
}
