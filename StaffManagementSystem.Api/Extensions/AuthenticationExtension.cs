using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StaffManagementSystem.Domain.Models;
using System.Security.Claims;
using System.Text;

namespace StaffManagementSystem.Api.Extensions {
    public static class AuthenticationExtension {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services) {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER")!,
                    ValidAudience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE")!,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")!))
                };
                options.Events = new JwtBearerEvents {
                    OnTokenValidated = async context => {
                        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
                        var principal = context.Principal!;

                        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var tokenStamp = principal.FindFirst("security_stamp")?.Value;

                        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenStamp)) {
                            context.Fail("Invalid token");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId);

                        if (user == null) context.Fail("User not found");
                        else if (!user.IsActive) context.Fail("User is inactive");
                        else if (user.SecurityStamp != tokenStamp) context.Fail("Token revoked");
                    }
                };
            });

            return services;
        }
    }
}
