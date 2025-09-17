using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Application.Options;
using Microsoft.AspNetCore.Http;

namespace CraneCalc.Application.Services;

public class TokenService(IHttpContextAccessor httpContextAccessor) : ITokenService
{
    public string? GetCurrentUserLogin()
    {
        var token = httpContextAccessor.HttpContext?.Request.Cookies[TokenName.Cookie];
        
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenRead = tokenHandler.ReadJwtToken(token);
        
        return tokenRead.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
    }
}