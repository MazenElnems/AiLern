using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Courses.Shared.DTO;
using LMS.Application.Settings;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.Courses.Queries.GetAIResources;

public class GetAIResourcesCommandHandler : IRequestHandler<GetAIResourcesCommand, Result<List<AiFileDto>>>
{
    private readonly IUserContext _user;
    private readonly IUnitOfWork _unitOfWork;
    private readonly BunnyOptions _bunnyOptions;
    private readonly IBunnyUrlSigner _bunnyUrl;

    public GetAIResourcesCommandHandler(IUserContext user, IUnitOfWork unitOfWork, IOptions<BunnyOptions> bunnyOptions, IBunnyUrlSigner bunnyUrl)
    {
        _user = user;
        _unitOfWork = unitOfWork;
        _bunnyOptions = bunnyOptions.Value;
        _bunnyUrl = bunnyUrl;
    }

    public async Task<Result<List<AiFileDto>>> Handle(GetAIResourcesCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.GetCurrentUser().Id;
        var course =  await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return DomainErrors.Course.NotFound(request.CourseId);
        }
        if (course.InstructorId != userId)
        {
            return DomainErrors.Course.NotOwned;
        }
        var aiResources = await _unitOfWork.AIResources.Query
            .AsNoTracking()
            .Where(r => r.CourseId == request.CourseId)
            .Select(f => new AiFileDto
            {
                Id = f.Id,
                FileUrl = f.StoragePath,
                FileName = f.FileName,
                FileSize = f.FileSize,
                ContentType = f.FileType
            }).ToListAsync(cancellationToken);
        aiResources.ForEach(r =>
        {
            r.FileUrl = _bunnyUrl.GenerateSignedUrl(_bunnyOptions.BaseUrl,_bunnyOptions.Token,r.FileUrl,TimeSpan.FromHours(1));
        });
        return aiResources;

    }
}
