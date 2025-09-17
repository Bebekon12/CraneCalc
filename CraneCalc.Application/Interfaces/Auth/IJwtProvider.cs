using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces.Auth;

public interface IJwtProvider
{
    public string GenerateToken(UserModel userModel);
}