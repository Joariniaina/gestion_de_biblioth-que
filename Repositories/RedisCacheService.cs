using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using LibraryApp.Web.Models;
using Microsoft.Extensions.Configuration;

namespace LibraryApp.Web.Repositories
{
    public class RedisCacheService
    {
        private readonly IDistributedCache _cache;
        public RedisCacheService(IDistributedCache cache) => _cache = cache;
    
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
            });
        }
    
        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _cache.GetStringAsync(key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json);
        }
    }
}