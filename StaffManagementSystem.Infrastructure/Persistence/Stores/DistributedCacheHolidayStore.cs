using Microsoft.Extensions.Caching.Distributed;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;
using System.Text.Json;

namespace StaffManagementSystem.Infrastructure.Persistence.Stores {
    public sealed class DistributedCacheHolidayStore(IDistributedCache cache) : IHolidayCacheStore {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private static string BuildKey(string calendarSourceId) => $"holidays:{calendarSourceId}";

        public async Task<IReadOnlyList<Holiday>?> GetAsync(
            string calendarSourceId,
            CancellationToken cancellationToken = default) {
            var json = await cache.GetStringAsync(BuildKey(calendarSourceId), cancellationToken);
            return json is null ? null : JsonSerializer.Deserialize<List<Holiday>>(json, JsonOptions);
        }

        public async Task SetAsync(
            string calendarSourceId,
            IReadOnlyList<Holiday> holidays,
            TimeSpan ttl,
            CancellationToken cancellationToken = default) {
            var json = JsonSerializer.Serialize(holidays, JsonOptions);

            await cache.SetStringAsync(
                BuildKey(calendarSourceId),
                json,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl },
                cancellationToken);
        }
    }
}
