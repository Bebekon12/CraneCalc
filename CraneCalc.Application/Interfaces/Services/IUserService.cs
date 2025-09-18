using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces.Services;

public interface IUserService
{
    string? GetCurrentUserLogin();
    Task<int> GetCurrentUserIdAsync(CancellationToken ct);
    Task<UserModel?> GetCurrentUserAsync(CancellationToken ct);
}