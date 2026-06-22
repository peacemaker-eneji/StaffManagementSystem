using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace StaffManagementSystem.Api.Extensions {
    public static class SwaggerExtension {
        public static IServiceCollection AddSwaggerDocs(this IServiceCollection services) {
            services.AddSwaggerGen(o => {
                o.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "Staff Management System API Docs",
                    Version = "v1",
                    Description = "An ASP.NET Core Web API documented via Swagger XML annotations."
                });

                foreach (var project in new string[] {"Api", "Application", "Domain", "Infrastructure"}) {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, $"StaffManagementSystem.{project}.xml");
                    o.IncludeXmlComments(xmlPath);
                }
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
