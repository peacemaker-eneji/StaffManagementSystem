using MediatR;
using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Application.Features.Users.Commands;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Api.Controllers {

    [ApiController]
    [Route("users")]
    public class UsersController : Controller {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateUser(CreateUserCommand request) {
            var response = await _mediator.Send(request);
            return StatusCode(response.Status, response);
        }

        [HttpPatch]
        public async Task<ActionResult<ApiResponse>> UpdateUser(UpdateUserCommand request) {
            var response = await _mediator.Send(request);
            return StatusCode(response.Status, response);
        }
    }
}
