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
}