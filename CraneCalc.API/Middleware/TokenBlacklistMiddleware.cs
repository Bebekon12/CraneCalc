using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Application.Options;

namespace CraneCalc.Api.Middleware;

public class TokenBlacklistMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITokenStorage tokenStorage)
    {
        if (IsPublicEndpoint(context.Request.Path))
        {
            await next(context);
            return;
        }

        var token = context.Request.Cookies[CookieNames.AccessToken];
        if (!string.IsNullOrEmpty(token))
        {
            if (await tokenStorage.IsTokenBlacklistedAsync(token, context.RequestAborted))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token is invalidated");
                return;
            }
        }

        await next(context);
    }

    private static bool IsPublicEndpoint(PathString path)
    {
        var publicPaths = new[]
        {
            "/api/user/login",
            "/api/user/register",
            "/api/cargo/paginated",
            "/api/cargo",
            "/api/crane-order/info",
        };

        return publicPaths.Any(p => path.StartsWithSegments(p));
    }
}