using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results.Generic;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.ChangePhoto;

public class ChangeUserPhotoCommand : IRequest<Result<string>>
{
    public FileMetaData Image { get; set; }
}
