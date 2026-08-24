using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Application.Features.Calendar.Queries {
    // Reads holidays from cache with optional filters. This is the same query used by
    // both the REST endpoint and the AI tool.
    public record GetHolidaysQuery(
        string? CalendarSourceId,
        DateOnly? From,
        DateOnly? To) : IRequest<ApiResponse<List<HolidayDto>>>;

    public record HolidayDto(
        string Id,
        string Name,
        string? Description,
        DateOnly StartDate,
        DateOnly? EndDate,
        bool IsAllDay,
        string? Location);

    public class GetHolidaysQueryHandler : IRequestHandler<GetHolidaysQuery, ApiResponse<List<HolidayDto>>> {
        private readonly IAppDbContext _context;
        private readonly IHolidayCacheStore _holidayCacheStore;


        public GetHolidaysQueryHandler(IAppDbContext context, IHolidayCacheStore holidayCacheStore) {
            _context = context;
            _holidayCacheStore = holidayCacheStore;
        }

        public async Task<ApiResponse<List<HolidayDto>>> Handle(GetHolidaysQuery request, CancellationToken cancellationToken = default) {
            var sourceIds = request.CalendarSourceId is not null
                ? new[] { request.CalendarSourceId }
                : await _context.CalendarSources.Select(s => s.Id).ToArrayAsync();

            var results = new List<HolidayDto>();

            foreach (var sourceId in sourceIds) {
                var holidays = await _holidayCacheStore.GetAsync(sourceId, cancellationToken);
                if (holidays is null) {
                    // Cache miss - the background job hasn't populated this source yet.
                    continue;
                }

                var filtered = holidays.Where(h =>
                    (request.From is null || h.StartDate >= request.From) &&
                    (request.To is null || h.StartDate <= request.To));

                results.AddRange(filtered.Select(h => new HolidayDto(
                    h.Id, h.Name, h.Description, h.StartDate, h.EndDate, h.IsAllDay, h.Location)));
            }

            return new ApiResponse<List<HolidayDto>> {
                Status = StatusCodes.Status200OK,
                Message = "Fetch Holidays Successfully",
                Data = results.OrderBy(h => h.StartDate).ToList()
            };
        }
    }
}
