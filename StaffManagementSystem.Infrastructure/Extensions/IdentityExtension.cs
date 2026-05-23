using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StaffManagementSystem.Domain.Models;
using StaffManagementSystem.Infrastructure.Persistence;

namespace StaffManagementSystem.Infrastructure.Extensions {
    public static class IdentityExtension {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services) {
            services.Configure<IdentityOptions>(options => {
                // Disable all password complexity rules
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
            });

            services.AddIdentityCore<User>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
