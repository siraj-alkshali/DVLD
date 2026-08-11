using DVLD.API.Services;
using DVLD.API.Services.Interfaces;

namespace DVLD.API.Extensions;

public static class ReferenceDataServiceExtensions
{
    public static IServiceCollection AddReferenceDataServices(this IServiceCollection services)
    {
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IGenderService, GenderService>();
        services.AddScoped<ILicenseClassService, LicenseClassService>();
        services.AddScoped<ITestTypeService, TestTypeService>();
        services.AddScoped<IApplicationTypeService, ApplicationTypeService>();
        services.AddScoped<IApplicationStatusService, ApplicationStatusService>();
        services.AddScoped<ILicenseIssueReasonService, LicenseIssueReasonService>();

        return services;
    }
}