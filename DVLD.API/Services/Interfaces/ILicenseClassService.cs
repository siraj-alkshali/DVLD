using DVLD.API.DTOs.LicenseClasses;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface ILicenseClassService
{
    Task<IEnumerable<LicenseClassDto>> GetAllLicenseClassesAsync();
    // Task<LicenseClass?> GetLicenseClassByIdAsync(int licenseClassId);
    Task<LicenseClassDto?> GetLicenseClassDtoByIdAsync(int licenseClassId);
    Task<bool> LicenseClassExistsAsync(int licenseClassId);
}