using Azure.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using StaffManagementSystem.Application.Features.Calendar.Queries;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;
using System.ComponentModel;

namespace StaffManagementSystem.Infrastructure.AiTools {
    public class HolidayTools {
        private readonly ILogger<HolidayTools> _logger;
        private readonly IHolidayCacheStore _holidayCacheStore;

        public HolidayTools(ILogger<HolidayTools> logger, IHolidayCacheStore holidayCacheStore) {
            _logger = logger;
            _holidayCacheStore = holidayCacheStore;
        }


        [Description("""
            Gets the list of holidays/calendar events available, optionally filtered by date range,
            a search term, or a specific calendar. Use this whenever the user asks about holidays,
            dates, or events instead of guessing - the model's own knowledge of holiday dates may be
            wrong or out of date. Dates must be in yyyy-MM-dd format.
            """)]
        public async Task<List<HolidayDto>> GetHolidays(
            [Description("Start date (yyyy-MM-dd), inclusive. Omit for no lower bound.")]
            string? from,
            [Description("End date (yyyy-MM-dd), inclusive. Omit for no upper bound.")]
            string? to) {

            _logger.LogInformation("Get holidays tools was call by the chatbot");

            DateOnly? From = null;
            DateOnly? To = null;

            if (DateOnly.TryParse(from, out DateOnly a)) From = a;
            if (DateOnly.TryParse(from, out DateOnly b)) To = b;

            var holidays = await _holidayCacheStore.GetAsync("28763381-fbdc-468b-b299-f09028af4e8a");
            var filtered = holidays.Where(h =>
                    (From is null || h.StartDate >= From) &&
                    (To is null || h.StartDate <= To));

            return filtered.Select(h => new HolidayDto(
                h.Id, h.Name, h.Description, h.StartDate, h.EndDate, h.IsAllDay, h.Location)).ToList();
        }
    }
}
