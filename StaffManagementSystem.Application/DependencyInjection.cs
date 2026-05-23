using Microsoft.Extensions.DependencyInjection;

namespace StaffManagementSystem.Application {
    public static class DependencyInjection {
        public static IServiceCollection AddApplication(this IServiceCollection services) {
            services.AddMediatR(config => config.RegisterServicesFromAssembly(ApplicationAssembly.Assembly));
            return services;
        }
    }
}
