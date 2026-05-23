using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StaffManagementSystem.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace StaffManagementSystem.Application.Features.Users.Commands {
    public record UpdateUserCommand(string? Firstname, string? Lastname, [Required][EmailAddress] string Email) : IRequest<ApiResponse>;

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResponse> {
        private readonly UserManager<User> _userManager;

        public UpdateUserCommandHandler(UserManager<User> userManager) {
            _userManager = userManager;
        }

        public async Task<ApiResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken) {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null) return new ApiResponse {
                Status = StatusCodes.Status404NotFound,
                Message = "User not Found",
                Success = false
            };

            if (request.Firstname is not null) user.Firstname = request.Firstname;
            if (request.Lastname is not null) user.Lastname = request.Lastname;
            await _userManager.UpdateAsync(user);
            return new ApiResponse {
                Status = StatusCodes.Status202Accepted,
                Message = "User updated successfully"
            };
        }
    }
}
