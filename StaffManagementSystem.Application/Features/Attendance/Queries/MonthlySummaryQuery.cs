
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Application.Helpers;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;
namespace StaffManagementSystem.Application.Features.Attendance.Queries
{
    public record MonthlySummaryQuery(
        int Month,
        int Year,
        int Page = 1,
        int PageSize = 10
    ) : IRequest<ApiResponse<PagedResult<MonthlySummaryDto>>>;
    public record MonthlySummaryDto(
        string UserId,
        string FullName,
        int Present,
        int Late,
        int Absent,
        int TotalDays,
        double TotalHoursWorked
    );
    public class MonthlySummaryHandler : IRequestHandler<MonthlySummaryQuery, ApiResponse<PagedResult<MonthlySummaryDto>>>
    {
        private readonly IAppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly CacheService _cache;
        public MonthlySummaryHandler(IAppDbContext context, UserManager<User> userManager, CacheService cache)
        {
            _context = context;
            _userManager = userManager;
            _cache = cache;
        }
        public async Task<ApiResponse<PagedResult<MonthlySummaryDto>>> Handle(MonthlySummaryQuery request, CancellationToken ct)
        {
            // 1. Validate month and year
            if (request.Month < 1 || request.Month > 12)
            {
                return new ApiResponse<PagedResult<MonthlySummaryDto>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Success = false,
                    Message = "Month must be between 1 and 12."
                };
            }
            // 2. Check cache first
            var cacheKey = $"monthly_summary_{request.Month}_{request.Year}_{request.Page}_{request.PageSize}";
            var cached = await _cache.GetAsync<PagedResult<MonthlySummaryDto>>(cacheKey);
            if (cached is not null)
            {
                return new ApiResponse<PagedResult<MonthlySummaryDto>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Monthly summary retrieved from cache.",
                    Data = cached
                };
            }
            // 3. Build date range for the month
            var from = new DateOnly(request.Year, request.Month, 1);
            var to = new DateOnly(request.Year, request.Month, DateTime.DaysInMonth(request.Year, request.Month));
            // 4. Get all active users
            var users = _userManager.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.Lastname)
                .ToList();
            var totalCount = users.Count;
            var pagedUsers = users
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            // 5. Get attendance records for this month
            var userIds = pagedUsers.Select(u => u.Id).ToList();
            var records = await _context.AttendanceRecords
                .Where(a => a.Date >= from && a.Date <= to && userIds.Contains(a.UserId!))
                .ToListAsync(ct);
            // 6. Calculate working days in the month
            int workingDays = Enumerable.Range(1, DateTime.DaysInMonth(request.Year, request.Month))
                .Select(day => new DateOnly(request.Year, request.Month, day))
                .Count(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday);
            // 7. Build summary for each user
            var summary = pagedUsers.Select(user => {
                var userRecords = records.Where(r => r.UserId == user.Id).ToList();
                int present = userRecords.Count(r => r.Status == AttendanceStatus.Present);
                int late = userRecords.Count(r => r.Status == AttendanceStatus.Late);
                int absent = workingDays - userRecords.Count;
                double hoursWorked = userRecords
                    .Where(r => r.ClockIn != null && r.ClockOut != null)
                    .Sum(r => (r.ClockOut!.Value - r.ClockIn!.Value).TotalHours);
                return new MonthlySummaryDto(
                    user.Id,
                    user.Firstname + " " + user.Lastname,
                    present,
                    late,
                    absent < 0 ? 0 : absent,
                    workingDays,
                    Math.Round(hoursWorked, 2)
                );
            }).ToList();
            // 8. Save to cache
            var result = new PagedResult<MonthlySummaryDto>(summary, totalCount, request.Page, request.PageSize);
            await _cache.SetAsync(cacheKey, result, minutes: 10);
            return new ApiResponse<PagedResult<MonthlySummaryDto>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Monthly summary retrieved successfully.",
                Data = result
            };
        }
    }
}
