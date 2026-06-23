using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Application.Features.Attendance.Queries {
    public record ExportAttendanceQuery(
        DateOnly? Date = null,
        string? UserId = null
    ) : IRequest<ApiResponse<string>>;

    public class ExportAttendanceQueryHandler : IRequestHandler<ExportAttendanceQuery, ApiResponse<string>> {
        private readonly IAppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;

        public ExportAttendanceQueryHandler(IAppDbContext context, IWebHostEnvironment env, UserManager<User> userManager) {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        public async Task<ApiResponse<string>> Handle(ExportAttendanceQuery request, CancellationToken ct) {

            if (request.UserId is not null) {
                var user = await _userManager.FindByIdAsync(request.UserId);

                if (user is null) return new ApiResponse<string> {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found",
                    Success = false
                };
            }

            var records = await _context.AttendanceRecords
                .Include(a => a.User)
                .Where(a => (request.UserId == null || a.UserId == request.UserId) && (request.Date == null || a.Date == request.Date))
                .OrderByDescending(a => a.Date)
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

            var filePath = Path.Combine(_env.WebRootPath, "Storage", "Exports", "Attendance", $"attendance_export_{Guid.NewGuid().ToString()}.csv");
            await MiniExcel.SaveAsAsync(filePath, records);
            return new ApiResponse<string> {
                Status = StatusCodes.Status200OK,
                Data = filePath
            };
        }
    }
}
