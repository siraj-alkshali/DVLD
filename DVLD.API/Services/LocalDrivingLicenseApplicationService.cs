using DVLD.API.Common.Constants;
using DVLD.API.Common.Results;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class LocalDrivingLicenseApplicationService : ILocalDrivingLicenseApplicationService
{
    private readonly DVLDContext _context;

    public LocalDrivingLicenseApplicationService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<LocalDrivingLicenseApplication?> GetLocalDrivingLicenseApplicationById(int localDrivingAppId)
    {
        return await _context.LocalDrivingLicenseApplications
        .Include(localApp => localApp.BaseApplication)
        .ThenInclude(baseApp => baseApp.ApplicantPerson)
        .SingleOrDefaultAsync(localApp => localApp.LocalDrivingLicenseApplicationID == localDrivingAppId);
    }

    public bool IsActive(LocalDrivingLicenseApplication localDrivingApp)
    {
        return localDrivingApp.BaseApplication.ApplicationStatusID == (int)enApplicationStatus.New;
    }
}