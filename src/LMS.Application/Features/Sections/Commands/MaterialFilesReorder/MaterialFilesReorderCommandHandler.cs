using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Constants;
using LMS.Domain.DTOs.MaterialFiles;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LMS.Application.Features.Sections.Commands.MaterialFilesReorder
{
    public class MaterialFilesReorderCommandHandler : IRequestHandler<MaterialFilesReorderCommand, Result>
    {
        private readonly IUserContext _userContext;
        private readonly IUnitOfWork _unitOfWork;

        public MaterialFilesReorderCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork)
        {
            _userContext = userContext;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(MaterialFilesReorderCommand request, CancellationToken cancellationToken)
        {
            var user = _userContext.GetCurrentUser();

            var section = await _unitOfWork.Sections
                .GetAsync(sec => sec.Id == request.sectionId, [nameof(Section.MaterialFiles), nameof(Section.Course)]);

            if (section == null)
                return Result.Failure(Domain.Common.Errors.DomainErrors.Section.NotFound(request.sectionId));

            var course = section.Course;

            if (course.InstructorId != user.Id)
                return Result.Failure(
                    Domain.Common.Errors.DomainErrors.Common.Forbidden("You are not assigned to this course, so you can’t access its materials."));

            var files = section.MaterialFiles;

            if(!files.Any())
                return Result.Failure(Domain.Common.Errors.DomainErrors.Section.Empty);

            if (!request.OrderedFilesIds.All(id => files.Any(file=>file.Id == id)))
                return Result.Failure(Domain.Common.Errors.DomainErrors.Common
                    .BusinessRule("Invalid Files Reorder Request", "One or more files in the reorder request do not belong to this section."));

            for(int i =0; i < request.OrderedFilesIds.Count; i++)
            {
                var file = files.First(f => f.Id == request.OrderedFilesIds[i]);
                file.OrderIndex = i+1;
            }

            await _unitOfWork.CommitAsync();

            return Result.Success("Section files have been reordered successfully.");


        }
    }
}
