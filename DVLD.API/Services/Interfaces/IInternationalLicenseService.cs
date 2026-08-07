using DVLD.API.DTOs;

namespace DVLD.API.Services.Interfaces;

public interface IInternationalLicenseService
{
    Task<InternationalLicenseDto?> GetInternationalLicenseDtoByIdAsync(int internationalLicenseId);
    Task<bool> ActiveInternationalLicenseExistsAsync(int driverId);
}