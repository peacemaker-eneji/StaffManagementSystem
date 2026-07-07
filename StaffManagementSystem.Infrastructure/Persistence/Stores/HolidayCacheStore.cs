using Microsoft.Extensions.Caching.Distributed;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;
using System.Text.Json;

namespace StaffManagementSystem.Infrastructure.Persistence.Stores {
    public class HolidayCacheStore : IHolidayCacheStore {
        private readonly IDistributedCache _cache;
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
        private static string BuildKey(string calendarSourceId) => $"holidays:{calendarSourceId}";

        public HolidayCacheStore(IDistributedCache cache) {
            _cache = cache;
        }

        public async Task<IReadOnlyList<Holiday>?> GetAsync(
            string calendarSourceId,
            CancellationToken cancellationToken = default) {
            var json = await _cache.GetStringAsync(BuildKey(calendarSourceId), cancellationToken);
            return json is null ? null : JsonSerializer.Deserialize<List<Holiday>>(json, JsonOptions);
        }

        public async Task SetAsync(
            string calendarSourceId,
            IReadOnlyList<Holiday> holidays,
            TimeSpan ttl,
            CancellationToken cancellationToken = default) {
            var json = JsonSerializer.Serialize(holidays, JsonOptions);

            await _cache.SetStringAsync(
                BuildKey(calendarSourceId),
                json,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl },
                cancellationToken);
        }
    }
}
