using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Application.Features.Attendance.Queries {
    public record LateArrivalsReportQuery(
        DateOnly? From = null,
        DateOnly? To = null,
        int Page = 1,
        int PageSize = 10
    ) : IRequest<ApiResponse<PagedResult<LateArrivalDto>>>;

    public record LateArrivalDto(
        string UserId,
        string FullName,
        DateOnly Date,
        TimeOnly ClockIn,
        int MinutesLate
    );
    public class LateArrivalsReportHandler : IRequestHandler<LateArrivalsReportQuery, ApiResponse<PagedResult<LateArrivalDto>>> {
        private readonly IAppDbContext _context;

        public LateArrivalsReportHandler(IAppDbContext context) {
            _context = context;
        }

        public async Task<ApiResponse<PagedResult<LateArrivalDto>>> Handle(LateArrivalsReportQuery request, CancellationToken ct) {
            var to = request.To;
            var from = request.From;
            var now = DateTime.Now;

            if (from is null) from = new DateOnly(now.Year, 1, 1);
            if (to is null) to = DateOnly.FromDateTime(DateTime.Now);

            if (to < from) return new ApiResponse<PagedResult<LateArrivalDto>> {
                Status = StatusCodes.Status406NotAcceptable,
                Message = "From date after To date is not allowed",
                Success = false
            };

            var query = _context.AttendanceRecords
                .Include(a => a.User)
                .Where(a =>
                    a.Date >= from &&
                    a.Date <= to &&
                    a.Status==AttendanceStatus.Late &&
                    a.ClockIn != null)
                .OrderByDescending(a => a.Date)
                    .ThenBy(a => a.User!.Lastname);

            var totalCount = await query.CountAsync(ct);

            if (totalCount == 0) return new ApiResponse<PagedResult<LateArrivalDto>> {
                Status = StatusCodes.Status204NoContent,
                Message = "No late arrivals found for the given range."
            };

            var records = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(a => new LateArrivalDto(
                    a.UserId!,
                    a.User!.Firstname + " " + a.User.Lastname,
                    a.Date,
                    a.ClockIn!.Value,
                    (int)(a.ClockIn!.Value - AttendanceRecord.GraceEnd).TotalMinutes
                ))
                .ToListAsync(ct);

            return new ApiResponse<PagedResult<LateArrivalDto>> {
                Status = StatusCodes.Status200OK,
                Message = "Late arrivals report retrieved.",
                Data = new PagedResult<LateArrivalDto>(records, totalCount, request.Page, request.PageSize)
            };
        }
    }
}
