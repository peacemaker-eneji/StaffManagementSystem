using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffManagementSystem.Domain.Interfaces;

namespace StaffManagementSystem.Application {
    public static class DependencyInjection {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config) {
            services.AddMediatR(config => config.RegisterServicesFromAssembly(ApplicationAssembly.Assembly));
            return services;
        }
    }
}
