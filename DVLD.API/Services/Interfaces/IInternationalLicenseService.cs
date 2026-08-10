using DVLD.API.Common.QueryParameters;
using DVLD.API.DTOs;
using DVLD.API.DTOs.Common;

namespace DVLD.API.Services.Interfaces;

public interface IInternationalLicenseService
{
    Task<InternationalLicenseDto?> GetInternationalLicenseDtoByIdAsync(int internationalLicenseId);
    Task<PagedResultDto<InternationalLicenseDto>> GetAllInternationalLicensesAsync(InternationalLicensesQueryParameters parameters);
    Task<bool> ActiveInternationalLicenseExistsAsync(int driverId);
}