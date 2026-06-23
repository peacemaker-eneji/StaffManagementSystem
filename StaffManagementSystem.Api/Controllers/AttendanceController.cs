using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Application.Features.Attendance.Commands;
using StaffManagementSystem.Application.Features.Attendance.Queries;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Api.Controllers {
    [Authorize]
    [ApiController]
    [Route("attendance")]
    public class AttendanceController : ControllerBase {
        private readonly IMediator _mediator;

        public AttendanceController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost("check-in/{id}")]
        public async Task<ActionResult<ApiResponse<CheckInResponse>>> CheckIn(string id) {
            var response = await _mediator.Send(new CheckInCommand(id));
            return StatusCode(response.Status, response);
        }
        
        [HttpPost("check-out/{id}")]
        public async Task<ActionResult<ApiResponse<CheckOutResponse>>> CheckOut(string id) {
            var response = await _mediator.Send(new CheckOutCommand(id));
            return StatusCode(response.Status, response);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<AttendanceRecordDto>>>> GetAttendance([FromQuery] GetAttendanceQuery request) {
            var response = await _mediator.Send(request);
            return StatusCode(response.Status, response);
        }

        [HttpGet("daily-attendance-report")]
        public async Task<ActionResult<ApiResponse<PagedResult<DailyAttendanceReportDto>>>> GetDailyAttendanceReport([FromQuery] DailyAttendanceReportQuery request) {
            var response = await _mediator.Send(request);
            return StatusCode(response.Status, response);
        }

        [HttpGet("late-arrivals-report")]
        public async Task<ActionResult<ApiResponse<PagedResult<LateArrivalDto>>>> GetLateArrivalsReport([FromQuery] LateArrivalsReportQuery request) {
            var response = await _mediator.Send(request);
            return StatusCode(response.Status, response);
        }
    }
}
