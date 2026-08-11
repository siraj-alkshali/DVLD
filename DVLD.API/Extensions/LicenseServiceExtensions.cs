using DVLD.API.Services;
using DVLD.API.Services.Interfaces;

namespace DVLD.API.Extensions;

public static class LicenseServiceExtensions
{
    public static IServiceCollection AddLicenseServices(
        this IServiceCollection services)
    {
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IImageService, ImageService>();

        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<ILicenseService, LicenseService>();
        services.AddScoped<IDetainedLicenseService, DetainedLicenseService>();
        services.AddScoped<IInternationalLicenseService, InternationalLicenseService>();

        return services;
    }
}