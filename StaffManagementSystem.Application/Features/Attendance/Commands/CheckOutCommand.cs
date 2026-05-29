using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Application.Features.Attendance.Commands {
    public record CheckOutCommand(string UserId) : IRequest<ApiResponse<CheckOutResponse>>;

    public record CheckOutResponse(
        string AttendanceId,
        string UserId,
        DateOnly Date,
        TimeOnly ClockIn,
        TimeOnly ClockOut,
        AttendanceStatus Status,
        bool IsHalfDay,
        double HoursWorked
    );
    public class CheckOutCommandHandler : IRequestHandler<CheckOutCommand, ApiResponse<CheckOutResponse>> {
        private readonly IAppDbContext _context;

        public CheckOutCommandHandler(IAppDbContext context) {
            _context = context;
        }

        public async Task<ApiResponse<CheckOutResponse>> Handle(CheckOutCommand request, CancellationToken ct) {

            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = TimeOnly.FromDateTime(DateTime.Now);

            var record = await _context.AttendanceRecords
                .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.Date == today, ct);

            if (record is null) return new ApiResponse<CheckOutResponse> {
                Status = StatusCodes.Status403Forbidden,
                Success = false,
                Message = "No clock-in record found for today."
            };

            // Domain method handles all business rules and throws DomainException on violations
            try {
                record.CheckOut(now);
            } catch (DomainException ex) {
                return new ApiResponse<CheckOutResponse> {
                    Status = StatusCodes.Status403Forbidden,
                    Success = false,
                    Message = ex.Message
                };
            }

            await _context.SaveChangesAsync(ct);

            var hoursWorked = (record.ClockOut!.Value - record.ClockIn!.Value).TotalHours;

            var response = new CheckOutResponse(
                AttendanceId: record.Id,
                UserId: record.UserId!,
                Date: record.Date,
                ClockIn: record.ClockIn.Value,
                ClockOut: record.ClockOut.Value,
                Status: record.Status,
                IsHalfDay: record.IsHalfDay,
                HoursWorked: Math.Round(hoursWorked, 2)
            );

            return new ApiResponse<CheckOutResponse> {
                Status = StatusCodes.Status202Accepted,
                Message = "Clocked Out Successfully",
                Data = response
            };
        }
    }
}
