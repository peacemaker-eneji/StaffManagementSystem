using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace StaffManagementSystem.Api.Extensions {
    public static class SwaggerExtension {
        public static IServiceCollection AddSwaggerDocs(this IServiceCollection services) {
            services.AddSwaggerGen(o => {
                o.UseInlineDefinitionsForEnums();
                o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme {
                    Name = "Authorization",
                    Description = "Enter your token in this field",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                });
                o.AddSecurityRequirement(new OpenApiSecurityRequirement { {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },[]}
                });
            });

            return services;
        }
    }
}
