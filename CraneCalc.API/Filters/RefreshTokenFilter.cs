using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CraneCalc.Application.Interfaces.Auth;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CraneCalc.API.Configurations;

public class RefreshTokenFilter(
    IJwtProvider jwtProvider,
    ITokenStorage tokenStorage,
    IUserRepository userRepository)
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("X-Refresh-Token", out var value))
        {
            await next();
            return;
        }

        try
        {
            var refreshToken = value.ToString();
            var userId = GetUserIdFromRequest(context);

            if (!userId.HasValue)
            {
                await next();
                return;
            }

            if (await IsValidRefreshToken(userId.Value, refreshToken))
            {
                var user = await userRepository.GetUserByIdAsync(userId.Value, context.HttpContext.RequestAborted);
                
                if (user == null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
                
                var newAccessToken = jwtProvider.GenerateToken(user);
                var newRefreshToken = jwtProvider.GenerateRefreshToken();

                await tokenStorage.SaveRefreshTokenAsync(
                    userId.Value, 
                    newRefreshToken, 
                    TimeSpan.FromDays(7),
                    context.HttpContext.RequestAborted
                );

                context.HttpContext.Response.Headers["X-New-Access-Token"] = newAccessToken;
                context.HttpContext.Response.Headers["X-New-Refresh-Token"] = newRefreshToken;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in RefreshTokenFilter: {ex.Message}");
        }

        await next();
    }

    private async Task<bool> IsValidRefreshToken(Guid userId, string refreshToken)
    {
        var storedToken = await tokenStorage.GetRefreshTokenAsync(userId, CancellationToken.None);
        return storedToken == refreshToken;
    }

    private static Guid? GetUserIdFromRequest(ActionExecutingContext context)
    {
        try
        {
            var authHeader = context.HttpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            var token = authHeader["Bearer ".Length..].Trim();
            
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return null;

            return userId;
        }
        catch
        {
            return null;
        }
    }
}