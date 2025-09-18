using CraneCalc.Application.Interfaces.Services;
using MediatR;

namespace CraneCalc.Application.Features.User.Commands.Logout;

public class LogoutCommandHandler(ITokenStorage storage, IUserService service) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        var userId = await service.GetCurrentUserIdAsync(ct);
        
        await storage.RemoveRefreshTokenAsync(userId, ct);
    }
}