using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace StaffManagementSystem.Application.Features.Users.Commands {

    public record CreateUserCommand(string Firstname, string Lastname, [EmailAddress] string Email, string Password, Role Role) : IRequest<ApiResponse>;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse> {
        private readonly UserManager<User> _userManager;

        public CreateUserCommandHandler(UserManager<User> userManager) {
            _userManager = userManager;
        }

        public async Task<ApiResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken) {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is not null) return new ApiResponse {
                Status = StatusCodes.Status409Conflict,
                Message = "Email Already Exists",
                Success = false
            };

            user = new User {
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                UserName = request.Email,
                Email = request.Email,
                Role = request.Role
            };

            await _userManager.CreateAsync(user, request.Password);
            await _userManager.AddToRoleAsync(user, user.Role.ToString());
            return new ApiResponse {
                Status = StatusCodes.Status201Created,
                Message = "User Created Successfully"
            };
        }
    }
}
