using DVLD.API.Common.QueryParameters;
using DVLD.API.Common.Results;
using DVLD.API.DTOs;
using DVLD.API.DTOs.Applications;
using DVLD.API.DTOs.Common;
using DVLD.API.DTOs.DetainedLicenses;
using DVLD.API.DTOs.Licenses;
using DVLD.API.DTOs.TestAppointments;

namespace DVLD.API.Services.Interfaces;

public interface IApplicationService
{
    Task<PagedResultDto<ApplicationDto>> GetAllApplicationsAsync(ApplicationsQueryParameters parameters);
    Task<ApplicationDto?> GetApplicationDtoByIdAsync(int applicationId);
    Task<ServiceResult<ApplicationDto>> CreateNewDrivingLicenseApplicationAsync(CreateLocalDrivingLicenseApplicationDto createLocalDrivingLicenseApplicationDto);
    Task<ServiceResult<TestAppointmentDto>> CreateNewRetakeTestApplication(CreateRetakeTestApplicationDto createRetakeTestApplicationDto);
    Task<ServiceResult<LicenseDto>> RenewDrivingLicenseAsync(RenewLicenseDto renewLicenseDto);
    Task<ServiceResult<LicenseDto>> ReplaceDrivingLicenseAsync(ReplaceLicenseDto replaceLicenseDto);
    Task<ServiceResult<InternationalLicenseDto>> IssueInternationalLicenseAsync(IssueInternationalLicenseDto internationalLicenseDto);
    Task<ServiceResult<DetainedLicenseDto>> ReleaseDetainedLicense(ReleaseDetainedLicenseDto releaseDetainedLicenseDto);
}