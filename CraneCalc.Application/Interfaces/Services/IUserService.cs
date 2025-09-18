using CraneCalc.Domain.Models;

namespace CraneCalc.Application.Interfaces.Services;

public interface IUserService
{
    string? GetCurrentUserLogin();
    Task<Guid> GetCurrentUserIdAsync(CancellationToken ct);
    Task<UserModel?> GetCurrentUserAsync(CancellationToken ct);
}