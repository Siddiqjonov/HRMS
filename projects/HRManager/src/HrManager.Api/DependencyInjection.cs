using HrManager.Api.Infrastructure;
using HrManager.Application.Common.Services.EmailService;
using HrManager.Application.UseCases.AbsenceRequests.AbsenceRequestConfiguration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace HrManager.Api;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddExceptionHandler<ExceptionHandlerMiddleware>();

        builder.Services.AddOptions<EmailConfiguration>()
               .BindConfiguration("EmailConfiguration")
               .ValidateDataAnnotations()
               .ValidateOnStart();

        builder.Services.AddOptions<AbsencePoliciesConfig>()
            .BindConfiguration("AbsencePolicies")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services) 
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Hr Manager",
                Version = "v1",
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter your JWT token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme,
                        },
                    },
                    []
                },
            };

            options.AddSecurityRequirement(securityRequirement);
        });

        return services;
    }
}
