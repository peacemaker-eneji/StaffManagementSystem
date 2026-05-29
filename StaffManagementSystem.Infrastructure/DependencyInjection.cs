using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Infrastructure.Extensions;
using StaffManagementSystem.Infrastructure.Services;

namespace StaffManagementSystem.Infrastructure {
    public static class DependencyInjection {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) {
            services.AddIdentityServices();
            services.AddPersistence(config);
            services.AddScoped<IBulkImportService, BulkImportService>();
            return services;
        }
    }
}
