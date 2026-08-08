using DVLD.API.DTOs.Licenses;
using DVLD.API.Common.Results;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface ILicenseService
{
    Task<LicenseDto?> GetLicenseDtoById(int licenseId);
    Task<ServiceResult<LicenseDto>> CreateNewLicenseAsync(CreateLicenseDto localDrivingAppId);
    Task<License?> GetLicenseWithDetailsByIdAsync(int licenseId);
    Task<License?> GetLicenseByIdForDetentionAsync(int licenseId);
}