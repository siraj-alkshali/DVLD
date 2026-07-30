using DVLD.API.DTOs.LicenseClasses;

namespace DVLD.API.Services.Interfaces;

public interface ILicenseClassService
{
    Task<IEnumerable<LicenseClassDto>> GetAllLicenseClassesAsync();
    Task<LicenseClassDto?> GetLicenseClassByIdAsync(int id);
    Task<bool> LicenseClassExistsAsync(int id);
}