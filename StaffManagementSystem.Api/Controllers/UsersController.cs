using MediatR;
using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Application.Features.Users.Commands;
using StaffManagementSystem.Application.Features.Users.Queries;
using StaffManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Authorization;

namespace StaffManagementSystem.Api.Controllers {
    [Authorize]
    [ApiController]
    [Route("users")]
    public class UsersController : Controller {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator) {
            _mediator = mediator;
        }
        [Authorize(Roles = "Admin")]
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

        //[HttpDelete]
        //public async Task<ActionResult<ApiResponse>> DeleteUser(DeactivateUserCommand request) {
        //    return null!;
        //}
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginQuery request)
        {
            var response = await _mediator.Send(request);
            return StatusCode(response.Status, response);
        }
    }
}
