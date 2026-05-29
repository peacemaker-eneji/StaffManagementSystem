using StaffManagementSystem.Api.Extensions;
using System.Text.Json.Serialization;

namespace StaffManagementSystem.Api {
    static public class DependencyInjection {
        static public IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration config) {
            services.AddCors(options => options.AddPolicy("AllowSpecificOrigin", policy =>
                        policy.WithOrigins("https://localhost:7140")
                              .AllowAnyHeader()
                              .AllowAnyMethod()));

            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddEndpointsApiExplorer();
            services.AddJwtAuthentication();
            services.AddAuthorization();
            services.AddSwaggerDocs();
            services.AddHangfireService(config);

            return services;
        }
    }
}
