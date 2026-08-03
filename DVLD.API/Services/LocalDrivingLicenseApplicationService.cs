using DVLD.API.Common.Results;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services;

public class LocalDrivingLicenseApplicationService : ILocalDrivingLicenseApplicationService
{
    private readonly DVLDContext _context;
    private readonly ILicenseClassService _licenseClassService;

    public LocalDrivingLicenseApplicationService(DVLDContext context, ILicenseClassService licenseClassService)
    {
        _context = context;
        _licenseClassService = licenseClassService;
    }

    public async Task<ServiceResult<LocalDrivingLicenseApplication>> BuildLocalDrivingLicenseEntityAsync(int applicationId, int licenseClassId)
    {
        if (!await _licenseClassService.LicenseClassExistsAsync(licenseClassId))
            return ServiceResult<LocalDrivingLicenseApplication>.Failure(["This license class does not exist"], FailureType.Conflict);

        LocalDrivingLicenseApplication licenseApp = new LocalDrivingLicenseApplication
        {
            ApplicationID = applicationId,
            LicenseClassID = licenseClassId
        };

        return ServiceResult<LocalDrivingLicenseApplication>.Success(licenseApp);
    }
}