using Ical.Net.DataTypes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Application.Features.Calendar.Commands {
    public record RefreshCalendarCommand(string CalendarSourceId) : IRequest<ApiResponse<int>>;
    public class RefreshCalendarCommandHandler: IRequestHandler<RefreshCalendarCommand, ApiResponse<int>> {
        private readonly ILogger<RefreshCalendarCommandHandler> _logger;
        private readonly IAppDbContext _context;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IHolidayCacheStore _holidayStore;

        // How far back/forward to expand recurring events. wider windows mean bigger cache but fewer cache misses.
        private static readonly TimeSpan LookBack = TimeSpan.FromDays(30);
        private static readonly TimeSpan LookForward = TimeSpan.FromDays(365);

        public RefreshCalendarCommandHandler(IAppDbContext context, 
            ILogger<RefreshCalendarCommandHandler> logger,
            IHolidayCacheStore holidayCacheStore) {
            _context = context;
            _logger = logger;
            _holidayStore = holidayCacheStore;
        }


        public async Task<ApiResponse<int>> Handle(RefreshCalendarCommand request, CancellationToken ct) {
            var calendarSource = await _context.CalendarSources.FirstOrDefaultAsync(s => s.Id == request.CalendarSourceId, ct);

            if (calendarSource is null) return new ApiResponse<int> {
                Status = StatusCodes.Status404NotFound,
                Message = "Calendar source not found",
                Success = false
            };

            _logger.LogInformation("Refreshing calendar cache for {SourceId} from {Url}", calendarSource.Id, calendarSource.IcsUrl);

            using var response = await _httpClient.GetAsync(calendarSource.IcsUrl, ct);
            if (!response.IsSuccessStatusCode) return new ApiResponse<int> {
                Status = StatusCodes.Status503ServiceUnavailable,
                Message = "Unable to fetch from calendar source",
                Success = false
            };


            var rawIcs = await response.Content.ReadAsStringAsync(ct);

            var rangeStart = DateOnly.FromDateTime(DateTime.UtcNow.Subtract(LookBack));
            var rangeEnd = DateOnly.FromDateTime(DateTime.UtcNow.Add(LookForward));

            var holidays = ParseIcsContent(rawIcs, calendarSource.Id, rangeStart, rangeEnd);

            // TTL should comfortably outlive the Hangfire refresh interval so a slow/failed
            // fetch doesn't cause a cache miss right before the next scheduled refresh.
            var ttl = calendarSource.RefreshInterval + TimeSpan.FromHours(2);

            await _holidayStore.SetAsync(calendarSource.Id, holidays, ttl, ct);

            _logger.LogInformation("Cached {Count} holidays for source {SourceId} ({SourceName})", holidays.Count, calendarSource.Id, calendarSource.Name);

            return new ApiResponse<int> {
                Status = StatusCodes.Status200OK,
                Message = "Refreshed Calendar source successful",
                Data = holidays.Count
            };
        }


        public IReadOnlyList<Holiday> ParseIcsContent(string icsContent, string sourceCalendarId, DateOnly rangeStart, DateOnly rangeEnd) {
            var calendar = Ical.Net.Calendar.Load(icsContent);
            if (calendar is null) {
                return [];
            }

            var searchStart = new CalDateTime(rangeStart.ToDateTime(TimeOnly.MinValue));

            var holidays = new List<Holiday>();

            foreach (var calendarEvent in calendar.Events) {
                // GetOccurrences expands any recurrence rule for this event within range.
                var occurrences = calendarEvent.GetOccurrences(searchStart);

                foreach (var occurrence in occurrences) {
                    var start = occurrence.Period.StartTime;
                    var end = occurrence.Period.EndTime;

                    holidays.Add(new Holiday {
                        // Uid + start date keeps recurring instances distinct and stable
                        // across refreshes (important so the cache doesn't churn ids).
                        Id = $"{sourceCalendarId}:{calendarEvent.Uid}:{start.AsUtc:yyyyMMdd}",
                        Name = calendarEvent.Summary ?? "(No title)",
                        Description = calendarEvent.Description,
                        StartDate = DateOnly.FromDateTime(start.AsUtc),
                        EndDate = end is not null ? DateOnly.FromDateTime(end.AsUtc) : null,
                        IsAllDay = !start.HasTime,
                        Location = calendarEvent.Location,
                        SourceCalendarId = sourceCalendarId
                    });
                }
            }

            return holidays
                .OrderBy(h => h.StartDate)
                .ToList();
        }
    }
}

