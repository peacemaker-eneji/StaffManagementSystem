using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Models;
using StaffManagementSystem.Infrastructure.Persistence;

namespace StaffManagementSystem.Infrastructure.Jobs {
    public class AutoCheckOutJob {
        private readonly AppDbContext _context;
        private readonly ILogger<AutoCheckOutJob> _logger;

        public AutoCheckOutJob(AppDbContext context, ILogger<AutoCheckOutJob> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task RunAsync() {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var openRecords = await _context.AttendanceRecords
                .Where(a => a.Date == today
                    && a.ClockIn != null
                    && a.ClockOut == null
                    && a.Status != AttendanceStatus.Absent)
                .ToListAsync();

            if (!openRecords.Any()) return;

            foreach (var record in openRecords) {
                record.ClockOut = AttendanceRecord.CloseTime;
            }

            await _context.BulkUpdateAsync(openRecords);

            _logger.LogInformation("Closed {Count} open check-in records.", openRecords.Count);
        }
    }
}
