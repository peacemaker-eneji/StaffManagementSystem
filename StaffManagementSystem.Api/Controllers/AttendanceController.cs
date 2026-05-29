using MediatR;
using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Application.Features.Attendance.Commands;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Api.Controllers {
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
    }
}
