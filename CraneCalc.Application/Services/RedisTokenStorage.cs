using CraneCalc.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace CraneCalc.Application.Services;

public class RedisTokenStorage(IDistributedCache cache) : ITokenStorage
{
    public async Task<string?> GetRefreshTokenAsync(Guid userId, CancellationToken ct)
    {
        return await cache.GetStringAsync($"refresh_token:{userId}", ct);
    }

    public async Task SaveRefreshTokenAsync(Guid userId, string refreshToken, TimeSpan expiry, CancellationToken ct)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };
        
        await cache.SetStringAsync($"refresh_token:{userId}", refreshToken, options, ct);
    }

    public async Task RemoveRefreshTokenAsync(Guid userId, CancellationToken ct)
    {
        await cache.RemoveAsync($"refresh_token:{userId}", ct);
    }

    public async Task AddToBlacklistAsync(string token, TimeSpan expiry, CancellationToken ct)
    {
        var tokenHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };
        
        await cache.SetStringAsync($"blacklisted_token:{tokenHash}", "blacklisted", options, ct);
    }

    public async Task<bool> IsTokenBlacklistedAsync(string token, CancellationToken ct)
    {
        var tokenHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));
        var result = await cache.GetStringAsync($"blacklisted_token:{tokenHash}", ct);
        return result != null;
    }
}