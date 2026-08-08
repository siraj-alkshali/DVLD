using DVLD.API.DTOs.DetainedLicenses;
using DVLD.API.Common.Results;

namespace DVLD.API.Services.Interfaces;

public interface IDetainedLicenseService
{
    Task<DetainedLicenseDto?> GetDetainedLicenseDtoByIdAsync(int detainId);
    Task<ServiceResult<DetainedLicenseDto>> DetainLicenseAsync(DetainLicenseDto detainLicenseDto);
}