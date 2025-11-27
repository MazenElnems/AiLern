using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using LMS.Domin.RepositoriesInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Auth.UserRevokeRefreshTokenCommands;

public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILogger<RevokeRefreshTokenCommandHandler> _logger;

    public RevokeRefreshTokenCommandHandler(IUsersRepository usersRepository, ILogger<RevokeRefreshTokenCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _logger = logger;
    }

    public async Task Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var refreshToken = await _usersRepository.GetRefreshTokenAsync(request.RefresToken)
                ?? throw new ResourceNotFoundException(nameof(RefreshToken), request.RefresToken);

            refreshToken.RevokesOn = DateTime.UtcNow;
            await _usersRepository.CommitAsync();
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
