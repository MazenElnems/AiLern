using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCourseCommandHandler> _logger;
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;

    public CreateCourseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateCourseCommandHandler> logger, IUserContext userContext, IWasabiService wasabiService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _userContext = userContext;
        _wasabiService = wasabiService;
    }

    public async Task<Result<object>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var CodeNormalized = request.Code.Trim().ToUpper();
            var NameNormalized = request.Name.ToLower();

            var isExist = await _unitOfWork.Courses.AnyAsync(c => c.Code == CodeNormalized || c.Name == NameNormalized);

            if (isExist)
                return DomainErrors.Course.AlreadyExist;

            var currentUser = _userContext.GetCurrentUser();

            request.Code = CodeNormalized;
            request.Name = NameNormalized;
            string? key = null;
            string? url = null;
            if (request.Image != null && !request.Image.ContentType.StartsWith("image/"))
            {
                return DomainErrors.Common.BusinessRule("Invalid Image", "The uploaded file must be an image.");
            }
            if (request.Image != null )
            {
                key = $"courses/{request.Code}/photo/{Guid.NewGuid()}.{request.Image.FileName.Split('.').Last()}";
                url = await _wasabiService.GeneratePresignedUploadUrlAsync(key, request.Image.ContentType, 15, secret: false);
            }
            var course = _mapper.Map<Course>(request);

            course.InstructorId = currentUser.Id;
            course.CreatedAt = DateTime.UtcNow;
            course.ImageStoragePath = key;

            _logger.LogInformation("Creating new course {@Course}", request);

            await _unitOfWork.Courses.InsertAsync(course);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("new Course created successfully with ID: {courseId}", course.Id);
            return Result<object>.Success(new {CourseId = course.Id ,UploadImageUrl = url });
        }
        catch(Exception ex)
        {
            _logger.LogError("an error occurred while creating new course");
            throw;
        }
    }
}
