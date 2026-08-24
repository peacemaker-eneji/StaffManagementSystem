using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffManagementSystem.Application.Helpers;
using StaffManagementSystem.Domain.Interfaces;

namespace StaffManagementSystem.Application {
    public static class DependencyInjection {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config) {
            services.AddHttpContextAccessor();
            services.AddMediatR(config => config.RegisterServicesFromAssembly(ApplicationAssembly.Assembly));
            services.AddScoped<CacheService>();
            return services;
        }
    }
}
