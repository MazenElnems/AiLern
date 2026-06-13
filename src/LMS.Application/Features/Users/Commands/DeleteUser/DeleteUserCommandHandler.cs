using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Constants;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var admin = _userContext.GetCurrentUser();
        if (!admin.IsInRole(UserRoles.Admin))
            return DomainErrors.Common.Forbidden("You are not authorized to perform this action.");

        if (admin.Id == request.userId)
            return DomainErrors.Common.BusinessRule("Can't Do this",
                "You cannot delete your own account."
            );

        var user =await _unitOfWork.Users.GetByIdAsync(request.userId);

        if (user == null)
            return DomainErrors.User.NotFound(request.userId.ToString());


        _unitOfWork.Users.Delete(user);

        await _unitOfWork.CommitAsync();

        return Result.Success("User Deleted successfully From Database.");
    }
}
