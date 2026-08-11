using DVLD.API.Services;
using DVLD.API.Services.Interfaces;

namespace DVLD.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<ILocalDrivingLicenseApplicationService, LocalDrivingLicenseApplicationService>();
        services.AddScoped<ITestAppointmentService, TestAppointmentService>();
        services.AddScoped<ITestService, TestService>();

        return services;
    }
}