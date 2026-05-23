using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffManagementSystem.Infrastructure.Persistence;
using StaffManagementSystem.Infrastucture;

namespace StaffManagementSystem.Infrastructure.Extensions {
    public static class PersistenceExtension {
        
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config) {
            string assembly_name = InfrastructureAssembly.Assembly.GetName().Name!;

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(assembly_name)));

            return services;
        }
    }
}
