using DVLD.API.DTOs.LicenseIssueReasons;

namespace DVLD.API.Services.Interfaces;

public interface ILicenseIssueReasonService
{
    Task<IEnumerable<LicenseIssueReasonDto>> GetAllLicenseIssueReasonsAsync();
    Task<LicenseIssueReasonDto?> GetLicenseIssueReasonById(int licenseIssueReasonId);
}