using DVLD.API.DTOs.Licenses;
using DVLD.API.Common.Results;

namespace DVLD.API.Services.Interfaces;

public interface ILicenseService
{
    Task<ServiceResult<LicenseDto>> CreateNewLicenseAsync(CreateLicenseDto localDrivingAppId);
}