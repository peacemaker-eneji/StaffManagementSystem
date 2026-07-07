
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace StaffManagementSystem.Application.Helpers
{
    public class CacheService
    {
        private readonly IDistributedCache _cache;
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _cache.GetStringAsync(key);
            if (value is null) return default;
            return JsonSerializer.Deserialize<T>(value);
        }
        public async Task SetAsync<T>(string key, T value, int minutes = 5)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes)
            };
            var serialized = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serialized, options);
        }
    }
}