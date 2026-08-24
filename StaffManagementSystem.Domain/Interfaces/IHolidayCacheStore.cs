using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Domain.Interfaces {
    /// <summary>
    /// Read/write access to the cached, parsed holiday list for a calendar source.
    /// Implemented in Infrastructure on top of IDistributedCache.
    /// </summary>
    public interface IHolidayCacheStore {
        Task<IReadOnlyList<Holiday>?> GetAsync(string calendarSourceId, CancellationToken cancellationToken = default);

        Task SetAsync(
            string calendarSourceId,
            IReadOnlyList<Holiday> holidays,
            TimeSpan ttl,
            CancellationToken cancellationToken = default);
    }
}


