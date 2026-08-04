using DVLD.API.Common.Constants;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.Licenses;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.API.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class LicenseService : ILicenseService
{
    private readonly DVLDContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDriverService _driverService;
    private readonly ITestService _testService;

    public LicenseService(DVLDContext context, IDriverService driverService, ICurrentUserService currentUserService, ITestService testService)
    {
        _context = context;
        _driverService = driverService;
        _currentUserService = currentUserService;
        _testService = testService;
    }

    public async Task<ServiceResult<LicenseDto>> CreateNewLicenseAsync(CreateLicenseDto dto)
    {
        LocalDrivingLicenseApplication? localDrivingApp = await _context.LocalDrivingLicenseApplications
        .Include(localApp => localApp.BaseApplication)
        .ThenInclude(baseApp => baseApp.ApplicantPerson)
        .Include(localApp => localApp.LicenseClass)
        .Include(localApp => localApp.BaseApplication)
        .ThenInclude(baseApp => baseApp.License)
        .SingleOrDefaultAsync(localApp => localApp.LocalDrivingLicenseApplicationID == dto.LocalDrivingLicenseApplicationID);

        if (localDrivingApp == null)
            return ServiceResult<LicenseDto>.Failure(["This license application doesn't exist"], FailureType.Conflict);

        if (localDrivingApp.BaseApplication.ApplicationStatusID != (int)enApplicationStatus.New)
            return ServiceResult<LicenseDto>.Failure(["This license application is inactive"], FailureType.Conflict);

        if (localDrivingApp.BaseApplication.License != null)
            return ServiceResult<LicenseDto>.Failure(["This license application already has a license issued for it"], FailureType.Conflict);

        if (!await _testService.PassedAllRequiredTestsAsync(dto.LocalDrivingLicenseApplicationID))
            return ServiceResult<LicenseDto>.Failure(["This person didn't pass all of the required tests in order to be eligible for this license"], FailureType.Conflict);

        int? createdByUserId = _currentUserService.UserID;
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        if (createdByUserId is null)
            throw new InvalidOperationException("Current user ID or username is not available.");

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var (driver, isNew) = await _driverService.GetOrCreateDriverAsync(localDrivingApp.BaseApplication.ApplicantPersonID);

            if (isNew)
            {
                await _context.Drivers.AddAsync(driver);
                await _context.SaveChangesAsync();
            }

            License newLicense = new License
            {
                ApplicationID = localDrivingApp.ApplicationID,
                DriverID = driver.DriverID,
                LicenseClassID = localDrivingApp.LicenseClassID,
                IssueDate = today,
                ExpirationDate = today.AddYears(localDrivingApp.LicenseClass.DefaultValidityLength),
                Notes = dto.Notes,
                PaidFees = localDrivingApp.LicenseClass.ClassFees,
                IsActive = true,
                IssueReasonID = (int)enLicenseIssueReason.FirstTimeIssue,
                CreatedByUserID = createdByUserId.Value
            };

            await _context.Licenses.AddAsync(newLicense);
            localDrivingApp.BaseApplication.ApplicationStatusID = (int)enApplicationStatus.Completed;
            localDrivingApp.BaseApplication.LastStatusDate = today;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            LicenseDto resultDto = new LicenseDto(
                newLicense.LicenseID,
                $"{localDrivingApp.BaseApplication.ApplicantPerson.FirstName} {localDrivingApp.BaseApplication.ApplicantPerson.LastName}",
                localDrivingApp.BaseApplication.ApplicantPerson.NationalNo,
                localDrivingApp.BaseApplication.ApplicantPerson.Phone,
                localDrivingApp.LicenseClass.ClassName,
                ((enLicenseIssueReason)newLicense.IssueReasonID).GetDisplayName(),
                newLicense.IssueDate,
                newLicense.ExpirationDate,
                newLicense.IsActive
            );

            return ServiceResult<LicenseDto>.Success(resultDto);
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}