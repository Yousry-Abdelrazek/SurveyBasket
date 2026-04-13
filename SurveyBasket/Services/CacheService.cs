using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SurveyBasket.Services;

public class CacheService(IDistributedCache distributedCache) : ICacheService
{
    
    private readonly IDistributedCache _distributedCache = distributedCache;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var json = await _distributedCache.GetStringAsync(key, cancellationToken);

        return json is null 
            ? null 
            : JsonSerializer.Deserialize<T>(json);
    }
    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken) where T : class
    {
        await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value) , cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }


}
