using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Errors;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LMS.Domain.Common.Errors.DomainErrors;

namespace LMS.Application.Features.Sections.Commands.DeleteMaterialFile
{
    internal class DeleteMaterialFileCommandHandler : IRequestHandler<DeleteMaterialFileCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IWasabiService _wasabiService;

        public DeleteMaterialFileCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IWasabiService wasabiService)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _wasabiService = wasabiService;
        }

        public async Task<Result> Handle(DeleteMaterialFileCommand request, CancellationToken cancellationToken)
        {
            var user = _userContext.GetCurrentUser();

            var section = await _unitOfWork.Sections.GetAsync(sec => sec.Id == request.SectionId, [nameof(Domain.Entities.Section.Course), nameof(Domain.Entities.Section.MaterialFiles)]);

            if(section == null)
                return Result.Failure(DomainErrors.Section.NotFound(request.SectionId));

            if (section.Course.InstructorId != user.Id)
                return Result.Failure(DomainErrors.Common.Forbidden("You do not have permission to delete this section file."));

            var file = section.MaterialFiles.FirstOrDefault(f => f.Id == request.FileId);
            if (file == null)
                return Result.Failure(DomainErrors.MaterialFile.NotFound(request.FileId));

            var filePath = file.StoragePath;
            try
            {
                await _wasabiService.DeleteFileAsync(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete file from storage.", ex);
            }

            _unitOfWork.MaterialFiles.DeleteFile(file);
            await _unitOfWork.CommitAsync();
            return Result.Success();
        }
    }
}
