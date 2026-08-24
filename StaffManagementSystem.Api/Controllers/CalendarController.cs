using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Application.Features.Calendar.Queries;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Api.Controllers {

    [ApiController]
    [Route("calendar")]
    public class CalendarController : ControllerBase {
        private readonly IMediator _mediator;

        public CalendarController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpGet("holidays")]
        public async Task<ActionResult<ApiResponse<List<HolidayDto>>>> GetHolidays([FromQuery] GetHolidaysQuery request) {
            var response = await _mediator.Send(request);
            return StatusCode(response.Status, response);
        }

        /// <summary>
        /// Triggers Refresh Calendar Sources background Job
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("refresh-sources")]
        public async Task<ActionResult<ApiResponse>> RefreshSources() {
            // trigger job
            using (var connection = JobStorage.Current.GetConnection()) {
                var manager = new RecurringJobManager(JobStorage.Current);
                manager.TriggerJob("refresh-calendar-cache");
            }
            var response = new ApiResponse {
                Message = "Triggered job Successfully",
                Status = StatusCodes.Status200OK
            };
            return StatusCode(response.Status, response);
        }
    }
}
