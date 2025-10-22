using CraneCalc.Application.Features.User.Dto;
using CraneCalc.Application.Interfaces.Auth;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Application.Options;
using CraneCalc.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CraneCalc.Application.Features.User.Commands.Login;

public class LoginCommandHandler(
    IUserRepository repository, 
    IJwtProvider provider, 
    IPasswordHasher hasher,
    ITokenStorage tokenStorage,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<LoginCommand, AuthenticationResult?>
{
    public async Task<AuthenticationResult?> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await repository.GetUserByLoginAsync(request.Login, ct);
        
        if(user == null)
            throw new EntityException("User not found");
        
        var isCorrectUser = hasher.Verify(request.Password, user.Password);
        
        if(!isCorrectUser)
            throw new EntityException("Failed to login");
        
        var accessToken = provider.GenerateToken(user);
        var refreshToken = provider.GenerateRefreshToken();
        
        await tokenStorage.SaveRefreshTokenAsync(
            user.Id, 
            refreshToken, 
            TimeSpan.FromDays(7), 
            ct
        );

        SetTokenCookies(accessToken, refreshToken);
        
        return new AuthenticationResult(accessToken, refreshToken);
    }

    private void SetTokenCookies(string accessToken, string refreshToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        };

        httpContext.Response.Cookies.Append(CookieNames.AccessToken, accessToken, cookieOptions);

        cookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(7);
        httpContext.Response.Cookies.Append(CookieNames.RefreshToken, refreshToken, cookieOptions);
    }
}