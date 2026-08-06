using DVLD.API.Common.QueryParameters;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.Applications;
using DVLD.API.DTOs.Common;
using DVLD.API.DTOs.Licenses;
using DVLD.API.DTOs.TestAppointments;

namespace DVLD.API.Services.Interfaces;

public interface IApplicationService
{
    Task<PagedResultDto<ApplicationDto>> GetAllApplicationsAsync(ApplicationsQueryParameters parameters);
    Task<ServiceResult<ApplicationDto>> CreateNewDrivingLicenseApplicationAsync(CreateLocalDrivingLicenseApplicationDto createLocalDrivingLicenseApplicationDto);
    Task<ServiceResult<TestAppointmentDto>> CreateNewRetakeTestApplication(CreateRetakeTestApplicationDto createRetakeTestApplicationDto);
    Task<ServiceResult<LicenseDto>> RenewDrivingLicenseAsync(RenewLicenseDto renewLicenseDto);
}