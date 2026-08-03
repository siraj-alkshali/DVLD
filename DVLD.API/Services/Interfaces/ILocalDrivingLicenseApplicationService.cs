using DVLD.API.Common.Results;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface ILocalDrivingLicenseApplicationService
{
    Task<ServiceResult<LocalDrivingLicenseApplication>> BuildLocalDrivingLicenseEntityAsync(int applicationId, int licenseClassId);
}