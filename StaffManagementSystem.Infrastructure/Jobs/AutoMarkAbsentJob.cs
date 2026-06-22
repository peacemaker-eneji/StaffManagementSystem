using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Models;
using StaffManagementSystem.Infrastructure.Persistence;

namespace StaffManagementSystem.Infrastructure.Jobs {
    public class AutoMarkAbsentJob {
        private readonly AppDbContext _context;
        private readonly ILogger<AutoMarkAbsentJob> _logger;

        public AutoMarkAbsentJob(AppDbContext context, ILogger<AutoMarkAbsentJob> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task RunAsync() {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var absentEmployees = await _context.Users
                .Where(e => e.Role == Role.Employee && !_context.AttendanceRecords.Any(a => a.UserId == e.Id && a.Date == today))
                .ToListAsync();

            if (!absentEmployees.Any()) return;

            var absences = absentEmployees.Select(e => new AttendanceRecord {
                Id = Guid.NewGuid().ToString(),
                UserId = e.Id,
                Date = today,
                Status = AttendanceStatus.Absent
            }).ToList();

            await _context.BulkInsertAsync(absences);

            _logger.LogInformation("Marked {Count} employees as Absent.", absentEmployees.Count);
        }
    }
}
