using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Commands.Auth.UserRevokeRefreshTokenCommands;

public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RevokeRefreshTokenCommandHandler> _logger;

    public RevokeRefreshTokenCommandHandler(IUnitOfWork unitOfWork, ILogger<RevokeRefreshTokenCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var refreshToken = await _unitOfWork.Users.GetRefreshTokenAsync(request.RefresToken)
                ?? throw new ResourceNotFoundException(nameof(RefreshToken), request.RefresToken);

            refreshToken.RevokesOn = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();
        }
        catch(ResourceNotFoundException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("Unhandled exception occured while revoking refresh token");
            throw;
        }
    }
}
