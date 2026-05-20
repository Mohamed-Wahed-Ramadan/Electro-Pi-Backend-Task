using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ProjectManagement.Application.Interfaces;

namespace ProjectManagement.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var data = await _cache.GetStringAsync(key, cancellationToken);
        if (data == null)
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(data, JsonOptions);
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            // Cached payload shape may become stale after DTO/model changes.
            await _cache.RemoveAsync(key, cancellationToken);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };
        var json = JsonSerializer.Serialize(value, JsonOptions);
        await _cache.SetStringAsync(key, json, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // Prefix invalidation requires Redis server commands; simplified: no-op for distributed cache without SCAN
        // In production, use Redis IDatabase with key pattern scan
        return Task.CompletedTask;
    }
}
