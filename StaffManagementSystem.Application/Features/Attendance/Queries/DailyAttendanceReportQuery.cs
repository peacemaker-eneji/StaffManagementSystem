using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Application.Features.Attendance.Queries {
    public record DailyAttendanceReportDto (
        DateOnly Date,
        int Present,
        int Late,
        int Absent,
        int Total
        );

    public record DailyAttendanceReportQuery(
        DateOnly? From = null,
        DateOnly? To = null,
        int Page = 1,
        int PageSize = 10
        ) : IRequest<ApiResponse<PagedResult<DailyAttendanceReportDto>>>;


    public class DailyAttendanceReportHandler : IRequestHandler<DailyAttendanceReportQuery, ApiResponse<PagedResult<DailyAttendanceReportDto>>> {
        private readonly IAppDbContext _context;
        private readonly UserManager<User> _userManager;

        public DailyAttendanceReportHandler(IAppDbContext context, UserManager<User> userManager) {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApiResponse<PagedResult<DailyAttendanceReportDto>>> Handle(DailyAttendanceReportQuery request, CancellationToken ct) {
            var to = request.To;
            var from = request.From;
            var now = DateTime.Now;

            if (from is null) from = new DateOnly(now.Year, 1, 1);
            if (to is null) to = DateOnly.FromDateTime(DateTime.Now);

            if (to < from) return new ApiResponse<PagedResult<DailyAttendanceReportDto>> {
                Status = StatusCodes.Status406NotAcceptable,
                Message = "From date after To date is not acceptable",
                Success = false
            };

            var userJoinDates = _context.Users
                .Where(u => u.IsActive)
                .Select(u => DateOnly.FromDateTime(u.CreatedAt))
                .ToList();

            var records = await _context.AttendanceRecords
                .Where(r => from <= r.Date && r.Date <= to)
                .GroupBy(r => r.Date)
                .Select(g => new {
                    Date = g.Key,
                    Present = g.Count(r => r.Status == AttendanceStatus.Present),
                    Late = g.Count(r => r.Status == AttendanceStatus.Late),
                    Recorded = g.Count()

                })
                .OrderByDescending(r => r.Date)
                .ToListAsync();

            int days = to.Value.DayNumber - from.Value.DayNumber + 1;

            var pagedDates = Enumerable
                .Range(from.Value.DayNumber, days)
                .Select(n => DateOnly.FromDayNumber(n))
                .OrderByDescending(d => d)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var summary = pagedDates
                .Select(d => {
                    var record = records.FirstOrDefault(r => r.Date == d);
                    int total = userJoinDates.Count(jd => jd <= d);
                    return new DailyAttendanceReportDto(d, record?.Present ?? 0, record?.Late ?? 0, total - (record?.Recorded ?? 0), total);
                })
                .ToList();

            return new ApiResponse<PagedResult<DailyAttendanceReportDto>> {
                Status = StatusCodes.Status200OK,
                Message = "Daily Attendance Report Generated successfully",
                Data = new PagedResult<DailyAttendanceReportDto>(summary, days, request.Page, request.PageSize)
            };
        }
    } 
}
