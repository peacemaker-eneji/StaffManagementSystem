using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffManagementSystem.Application.Features.Calendar.Commands;
using StaffManagementSystem.Domain.Interfaces;

namespace StaffManagementSystem.Infrastructure.Jobs {
    public class RefreshCalendarCacheJob {
        private readonly ISender _mediator;
        private readonly IAppDbContext _context;
        private readonly ILogger<RefreshCalendarCacheJob> _logger;

        public RefreshCalendarCacheJob(ISender mediator, IAppDbContext context, ILogger<RefreshCalendarCacheJob> logger) {
            _mediator = mediator;
            _context = context;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken ct = default) {
            var sources = await _context.CalendarSources.Where(s => s.IsActive).ToListAsync();

            foreach (var source in sources) {
                var response = await _mediator.Send(new RefreshCalendarCommand(source.Id), ct);
                if (response.Success) {
                    _logger.LogInformation("Refreshed {Count} holidays for source {SourceId} ({SourceName})", response.Data, source.Id, source.Name);
                } else { 
                    _logger.LogError("Failed to refresh calendar cache for source {SourceId} ({SourceName})", source.Id, source.Name);
                }
            }
        }
    }

}
