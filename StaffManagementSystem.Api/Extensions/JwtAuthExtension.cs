using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace StaffManagementSystem.Api.Extensions {
    public static class JwtAuthExtension {
        public static IServiceCollection AddJwtAuth(this IServiceCollection services) {
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
            //    options.TokenValidationParameters = new TokenValidationParameters {
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = "https://localhost:7140",
            //        ValidAudience = "https://localhost:7140",
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")!))
            //    };
            //    options.Events = new JwtBearerEvents {
            //        OnTokenValidated = async context => {
            //            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
            //            var principal = context.Principal!;

            //            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //            var tokenStamp = principal.FindFirst("security_stamp")?.Value;

            //            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenStamp)) {
            //                context.Fail("Invalid token");
            //                return;
            //            }

            //            var user = await userManager.FindByIdAsync(userId);

            //            if (user == null) context.Fail("User not found");
            //            else if (!user.IsActive) context.Fail("User is inactive");
            //            else if (user.SecurityStamp != tokenStamp) context.Fail("Token revoked");
            //        }
            //    };
            //});

            return services;
        }
    }
}
