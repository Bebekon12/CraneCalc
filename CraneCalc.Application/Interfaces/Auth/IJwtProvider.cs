using System.Security.Claims;
using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces.Auth;

public interface IJwtProvider
{
    public string GenerateToken(UserModel userModel);

    public string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}