using LMS.Domain.Repositories;
using LMS.Application.Common.Results;
using LMS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Errors;

namespace LMS.Application.Features.Auth.Commands.RevokeToken;

public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RevokeRefreshTokenCommandHandler> _logger;

    public RevokeRefreshTokenCommandHandler(IUnitOfWork unitOfWork, ILogger<RevokeRefreshTokenCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var refreshToken = await _unitOfWork.Users.GetRefreshTokenAsync(request.RefresToken)
                ;
            if (refreshToken == null)
                return Result.Failure(DomainErrors.Auth.RefreshTokenNotFound(request.RefresToken));

            refreshToken.RevokesOn = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError("Unhandled exception occured while revoking refresh token");
            throw;
        }
    }
}
