using StaffManagementSystem.Api.Extensions;

namespace StaffManagementSystem.Api {
    static public class DependencyInjection {
        static public IServiceCollection AddPresentation(this IServiceCollection services) {
            services.AddCors(options => options.AddPolicy("AllowSpecificOrigin", policy =>
                        policy.WithOrigins("https://localhost:7140")
                              .AllowAnyHeader()
                              .AllowAnyMethod()));

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddJwtAuthentication();
            services.AddAuthorization();
            services.AddSwaggerDocs();

            return services;
        }
    }
}
