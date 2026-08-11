using DVLD.DataAccess.Data;

namespace DVLD.API.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services)
    {
        services.AddHealthChecks()
        .AddDbContextCheck<DVLDContext>(name: "DVLD-database", tags: ["database"]);

        return services;
    }
}