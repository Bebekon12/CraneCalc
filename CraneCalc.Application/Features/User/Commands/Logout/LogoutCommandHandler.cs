using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Application.Options;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CraneCalc.Application.Features.User.Commands.Logout;

public class LogoutCommandHandler(
    ITokenStorage storage, 
    IUserService service,
    IHttpContextAccessor httpContextAccessor,
    ILogger<LogoutCommandHandler> logger) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        var userId = await service.GetCurrentUserIdAsync(ct);
        
        var accessToken = GetAccessTokenFromCookie();
        if (!string.IsNullOrEmpty(accessToken))
        {
            try
            {
                await storage.AddToBlacklistAsync(accessToken, TimeSpan.FromHours(24), ct);
                logger.LogInformation("Access token blacklisted for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to blacklist token for user {UserId}", userId);
            }
        }

        ClearTokenCookies();
        
        await storage.RemoveRefreshTokenAsync(userId, ct);
    }

    private string? GetAccessTokenFromCookie()
    {
        return httpContextAccessor.HttpContext?.Request.Cookies[CookieNames.AccessToken];
    }

    private void ClearTokenCookies()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(-1)
        };

        httpContext.Response.Cookies.Append(CookieNames.AccessToken, "", cookieOptions);
        httpContext.Response.Cookies.Append(CookieNames.RefreshToken, "", cookieOptions);
    }
}