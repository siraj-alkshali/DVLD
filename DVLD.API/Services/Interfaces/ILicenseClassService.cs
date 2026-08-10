using DVLD.API.DTOs.LicenseClasses;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface ILicenseClassService
{
    Task<List<LicenseClassDto>> GetAllLicenseClassesAsync();
    Task<LicenseClassDto?> GetLicenseClassDtoByIdAsync(int licenseClassId);
    Task<bool> LicenseClassExistsAsync(int licenseClassId);
}