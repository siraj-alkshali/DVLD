using DVLD.API.Common.Results;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface ILocalDrivingLicenseApplicationService
{
    Task<LocalDrivingLicenseApplication?> GetLocalDrivingLicenseApplicationById(int id);
    bool IsActiveAsync(LocalDrivingLicenseApplication localDrivingApp);
}