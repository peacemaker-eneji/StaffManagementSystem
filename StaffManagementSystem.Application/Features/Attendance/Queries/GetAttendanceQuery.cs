using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Application.Features.Attendance.Queries {
    public record GetAttendanceQuery(
        DateOnly? Date = null,
        string? UserId = null,
        int Page = 1,
        int PageSize = 10
    ) : IRequest<ApiResponse<PagedResult<AttendanceRecordDto>>>;

    public record AttendanceRecordDto(
        string Id,
        string UserId,
        string FullName,
        DateOnly Date,
        TimeOnly? ClockIn,
        TimeOnly? ClockOut,
        AttendanceStatus Status,
        bool IsHalfDay,
        double? HoursWorked
    );

    public class GetAttendanceHandler : IRequestHandler<GetAttendanceQuery, ApiResponse<PagedResult<AttendanceRecordDto>>> {

        private readonly IAppDbContext _context;
        private readonly UserManager<User> _userManager;

        public GetAttendanceHandler(IAppDbContext context, UserManager<User> userManager) {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApiResponse<PagedResult<AttendanceRecordDto>>> Handle(GetAttendanceQuery request, CancellationToken ct) {
            DateOnly a = DateOnly.FromDateTime(DateTime.Now);

            if (request.UserId is not null) {
                var user = await _userManager.FindByIdAsync(request.UserId);

                if (user is null) return new ApiResponse<PagedResult<AttendanceRecordDto>> {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found",
                    Success = false
                };
            }

            var query = _context.AttendanceRecords
                .Include(a => a.User)
                .Where(a => (request.UserId == null || a.UserId == request.UserId) && (request.Date == null || a.Date == request.Date))
                .OrderByDescending(a => a.Date);

            var totalCount = await query.CountAsync(ct);

            var records = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(a => new AttendanceRecordDto(
                    a.Id,
                    a.UserId!,
                    a.User!.Firstname + " " + a.User.Lastname,
                    a.Date,
                    a.ClockIn,
                    a.ClockOut,
                    a.Status,
                    a.IsHalfDay,
                    a.ClockIn != null && a.ClockOut != null ? Math.Round((a.ClockOut.Value - a.ClockIn.Value).TotalHours, 2) : null
                ))
                .ToListAsync(ct);

            var result = new PagedResult<AttendanceRecordDto>(
                records,
                totalCount,
                request.Page,
                request.PageSize
            );

            return new ApiResponse<PagedResult<AttendanceRecordDto>> {
                Status = StatusCodes.Status200OK,
                Message = "Records retrieved.",
                Data = result
            };
        }
    }
}
