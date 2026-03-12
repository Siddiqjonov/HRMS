using HrManager.Application.Common.Behaviours;
using HrManager.Application.Common.Services;
using HrManager.Application.Common.Services.EmailService;
using HrManager.Domain.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;

namespace HrManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(Assembly.GetExecutingAssembly());
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;
            o.Audience = configuration["Keycloak:Audience"];
            o.MetadataAddress = configuration["Keycloak:MetadataAddress"]!;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = configuration["Keycloak:ValidIssuer"],
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policies.RequireAdmin,
                policy => policy.RequireRole(Roles.Admin));

            options.AddPolicy(
                Policies.RequireHrManager,
                policy => policy.RequireRole(Roles.HrManager));

            options.AddPolicy(
                Policies.RequireEmployee,
                policy => policy.RequireRole(Roles.Employee));

            options.AddPolicy(
                Policies.RequireAdminOrHrManager,
                policy => policy.RequireRole(Roles.Admin, Roles.HrManager));

            options.AddPolicy(
                Policies.RequireEmployeeOrHrManager,
                policy => policy.RequireRole(Roles.Employee, Roles.HrManager));
        });

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAbsenceBalanceService, AbsenceBalanceService>();
        services.AddScoped<IReportService, ReportService>();
        return services;
    }
}
