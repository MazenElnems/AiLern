using LMS.Core.Constants;
using LMS.Core.DTOs.Courses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Core.Queries.Students.GetAllQuery
{
    public class GetStudentCoursesQuery : IRequest<List<GetStudentCoursesDto>>
    {
        [JsonIgnore]
        public int Id { get; set; } 
        public string? SearchString { get; set; }
        public string? SortBy { get; set; }
        public string? Order { get; set; } = SortOrderOptions.DESC;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
