using LMS.Domain.Repositories;
using LMS.Application.Common.Results;
using MediatR;
using LMS.Domain.Errors;

namespace LMS.Application.Features.Auth.Commands.RevokeToken;

public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public RevokeRefreshTokenCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _unitOfWork.Users.GetRefreshTokenAsync(request.RefresToken);

        if (refreshToken == null)
            return DomainErrors.Auth.RefreshTokenNotFound(request.RefresToken);

        refreshToken.RevokesOn = DateTime.UtcNow;
        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
