using DVLD.API.DTOs.LicenseIssueReasons;

namespace DVLD.API.Services.Interfaces;

public interface ILicenseIssueReasonService
{
    Task<List<LicenseIssueReasonDto>> GetAllLicenseIssueReasonsAsync();
    Task<LicenseIssueReasonDto?> GetLicenseIssueReasonById(int licenseIssueReasonId);
}