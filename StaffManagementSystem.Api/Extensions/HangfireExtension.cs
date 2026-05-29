using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StaffManagementSystem.Domain.Models;
using System.Security.Claims;
using System.Text;

namespace StaffManagementSystem.Api.Extensions {
    public static class HangfireExtension {
        public static IServiceCollection AddHangfireService(this IServiceCollection services, IConfiguration config) {
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(config.GetConnectionString("DefaultConnection")));

            services.AddHangfireServer();

            return services;
        }
    }
}
