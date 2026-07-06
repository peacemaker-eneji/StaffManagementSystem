using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Infrastructure.Persistence;
using StaffManagementSystem.Infrastructure.Persistence.Stores;
using StaffManagementSystem.Infrastucture;

namespace StaffManagementSystem.Infrastructure.Extensions {
    public static class PersistenceExtension {
        
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config) {
            string assembly_name = InfrastructureAssembly.Assembly.GetName().Name!;

            services.AddDistributedMemoryCache();
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(assembly_name)));
            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddScoped<IHolidayCacheStore, DistributedCacheHolidayStore>();

            return services;
        }
    }
}
