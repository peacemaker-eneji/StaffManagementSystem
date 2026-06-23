
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StaffManagementSystem.Application.Helpers;
using StaffManagementSystem.Domain.Models;
namespace StaffManagementSystem.Application.Features.Users.Queries
{
    public record LoginQuery(string Email, string Password) : IRequest<ApiResponse<LoginResponse>>;
    public record LoginResponse(string Token);
    public class LoginQueryHandler : IRequestHandler<LoginQuery, ApiResponse<LoginResponse>>
    {
        private readonly UserManager<User> _userManager;
        public LoginQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<ApiResponse<LoginResponse>> Handle(LoginQuery request, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Success = false,
                    Message = "Invalid email or password."
                };
            }
            bool passwordIsCorrect = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordIsCorrect)
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Success = false,
                    Message = "Invalid email or password."
                };
            }
            string token = JwtTokenGenerator.GenerateToken(user);
            return new ApiResponse<LoginResponse>
            {
                Status = StatusCodes.Status200OK,
                Message = "Login successful.",
                Data = new LoginResponse(token)
            };
        }
    }
}