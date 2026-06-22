using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffManagementSystem.Infrastructure.Jobs;

namespace StaffManagementSystem.Infrastructure.Extensions {
    public static class JobExtension {
        
        public static IServiceCollection AddJobs(this IServiceCollection services) {
            services.AddScoped<AutoCheckOutJob>();
            services.AddScoped<AutoMarkAbsentJob>();

            return services;
        }
    }
}
