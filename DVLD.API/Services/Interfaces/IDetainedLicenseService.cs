using DVLD.API.DTOs.DetainedLicenses;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.Common;
using DVLD.API.Common.QueryParameters;

namespace DVLD.API.Services.Interfaces;

public interface IDetainedLicenseService
{
    Task<PagedResultDto<DetainedLicenseDto>> GetAllDetainedLicensesAsync(DetainedLicensesQueryParameters parameters);
    Task<DetainedLicenseDto?> GetDetainedLicenseDtoByIdAsync(int detainId);
    Task<ServiceResult<DetainedLicenseDto>> DetainLicenseAsync(DetainLicenseDto detainLicenseDto);
}