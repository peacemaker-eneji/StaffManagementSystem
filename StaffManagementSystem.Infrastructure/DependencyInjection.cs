using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffManagementSystem.Infrastructure.Extensions;

namespace StaffManagementSystem.Infrastructure {
    public static class DependencyInjection {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) {
            services.AddIdentityServices();
            services.AddPersistence(config);

            return services;
        }
    }
}
