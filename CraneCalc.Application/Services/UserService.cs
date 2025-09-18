using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Application.Interfaces.Services;
using CraneCalc.Application.Options;
using CraneCalc.Domain.Exceptions;
using CraneCalc.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CraneCalc.Application.Services;

public class UserService(IHttpContextAccessor httpContextAccessor, IUserRepository repository) : IUserService
{
    public string? GetCurrentUserLogin()
    {
        var token = httpContextAccessor.HttpContext?.Request.Cookies[CookieNames.AccessToken];
        
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenRead = tokenHandler.ReadJwtToken(token);
        
        return tokenRead.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
    }

    public async Task<Guid> GetCurrentUserIdAsync(CancellationToken ct)
    {
        var user  = await GetCurrentUserAsync(ct);
        
        if(user == null)
            throw new EntityException("User not found");
        
        return user.Id;
    }

    public async Task<UserModel?> GetCurrentUserAsync(CancellationToken ct)
    {
        var userLogin = GetCurrentUserLogin();
        
        if(userLogin == null)
            return null;
        
        var user = await repository.GetUserByLoginAsync(userLogin, ct);
        
        return user;
    }
}