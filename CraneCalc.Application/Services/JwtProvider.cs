using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CraneCalc.Application.Interfaces.Auth;
using CraneCalc.Application.Options;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CraneCalc.Application.Services;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateToken(UserModel userModel)
    {
        Claim[] claims = [
            new(ClaimTypes.Name, userModel.Login),
            new(ClaimTypes.Role, userModel.Role == Role.Moderator || userModel.Login=="bebekon" ? "Administrator" : "User")
        ];
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(_options.ExpiresHours)
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }
}