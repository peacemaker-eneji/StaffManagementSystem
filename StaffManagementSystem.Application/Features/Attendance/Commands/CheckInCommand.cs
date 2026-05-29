using Azure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Application.Features.Attendance.Commands {
    public record CheckInCommand(string UserId) : IRequest<ApiResponse<CheckInResponse>>;

    public record CheckInResponse(
        string AttendanceId,
        string UserId,
        DateOnly Date,
        TimeOnly ClockIn,
        AttendanceStatus Status
    );
    public class CheckInCommandHandler : IRequestHandler<CheckInCommand, ApiResponse<CheckInResponse>> {
        private readonly IAppDbContext _context;
        private readonly UserManager<User> _userManager;

        public CheckInCommandHandler(IAppDbContext context, UserManager<User> userManager) {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApiResponse<CheckInResponse>> Handle(CheckInCommand request, CancellationToken ct) {

            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = TimeOnly.FromDateTime(DateTime.Now);

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null || !user.IsActive) return new ApiResponse<CheckInResponse> {
                Status = StatusCodes.Status404NotFound,
                Success = false,
                Message = "User not found or inactive."
            };


            var existing = await _context.AttendanceRecords
                .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.Date == today, ct);

            if (existing is not null) {
                return new ApiResponse<CheckInResponse> {
                    Status = StatusCodes.Status403Forbidden,
                    Success = false,
                    Message = "Already clocked in for today."
                };
            }

            AttendanceRecord? record;
            try {
                record = AttendanceRecord.CheckIn(
                    userId: request.UserId,
                    date: today,
                    clockInTime: now
                );
            } catch (DomainException e) {
                return new ApiResponse<CheckInResponse> {
                    Status = StatusCodes.Status403Forbidden,
                    Message = e.Message,
                    Success = false
                };
            }

            await _context.AttendanceRecords.AddAsync(record, ct);
            await _context.SaveChangesAsync(ct);

            var response = new CheckInResponse(
                AttendanceId: record.Id,
                UserId: record.UserId!,
                Date: record.Date,
                ClockIn: record.ClockIn!.Value,
                Status: record.Status
            );
            return new ApiResponse<CheckInResponse> {
                Status = StatusCodes.Status202Accepted,
                Message = "clocked in successfully.",
                Data = response
            };
        }
    }
}
