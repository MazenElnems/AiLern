using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Enums;
using LMS.Domain.Common.Errors;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Sections.Commands.ConfirmFileUpload
{
    public class ConfirmMaterialUploadCommandHandler : IRequestHandler<ConfirmMaterialUploadCommand, Result>
    {
        private readonly IUserContext _userContext;
        private readonly IWasabiService _wasabiService;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmMaterialUploadCommandHandler(IUserContext userContext, IWasabiService wasabiService, IUnitOfWork unitOfWork)
        {
            _userContext = userContext;
            _wasabiService = wasabiService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ConfirmMaterialUploadCommand request, CancellationToken cancellationToken)
        {
            var user = _userContext.GetCurrentUser();

            var section = await _unitOfWork.Sections.GetAsync(sec => sec.Id == request.SectionId, [nameof(Section.Course),nameof(Section.MaterialFiles)]);

            if (section == null)
                return Result.Failure(DomainErrors.Section.NotFound(request.SectionId));
            var course = section.Course;

            if(course.InstructorId != user.Id)
                return Result.Failure(DomainErrors.Common.Forbidden("You do not have permission to request pre-signed URLs for this section."));

            foreach(var file in section.MaterialFiles)
            {
                var exist = await _wasabiService.FileExists(file.StoragePath);

                if(!exist)
                    return Result.Failure(DomainErrors.Storage.FileMissing);

                file.UploadStatus = UploadStatus.Completed;
            }

            await _unitOfWork.CommitAsync();
            return Result.Success();
        }
    }
}
