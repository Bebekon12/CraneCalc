namespace CraneCalc.Application.Interfaces.Services;

public interface ITokenStorage
{
    Task<string?> GetRefreshTokenAsync(Guid userId, CancellationToken ct);
    Task SaveRefreshTokenAsync(Guid userId, string refreshToken, TimeSpan expiry, CancellationToken ct);
    Task RemoveRefreshTokenAsync(Guid userId, CancellationToken ct);
}