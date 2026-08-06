namespace DVLD.API.Services.Interfaces;

public interface IDetainedLicenseService
{
    Task<bool> IsLicenseDetainedAsync(int licenseId);
}