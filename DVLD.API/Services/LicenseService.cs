using DVLD.API.Common.Constants;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.Licenses;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.API.Extensions;
using Microsoft.EntityFrameworkCore;
using DVLD.API.Common.QueryParameters;
using DVLD.API.DTOs.Common;

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

    public async Task<PagedResultDto<LicenseDto>> GetAllLicensesAsync(LicenseQueryParameters parameters)
    {
        IQueryable<License> query = _context.Licenses
        .AsNoTracking()
        .ApplySearch(parameters.SearchTerm)
        .ApplyFilter(parameters)
        .ApplySort(parameters);

        int totalItems = await query.CountAsync();

        List<LicenseDto> items = await query.ApplyPagination(parameters)
        .Select(l => new LicenseDto(
            l.LicenseID,
            $"{l.Application.ApplicantPerson.FirstName} {l.Application.ApplicantPerson.LastName}",
            l.Application.ApplicantPerson.NationalNo,
            l.Application.ApplicantPerson.Phone,
            l.LicenseClass.ClassName,
            l.LicenseIssueReason.IssueReasonName,
            l.IssueDate,
            l.ExpirationDate,
            l.IsActive
        )).ToListAsync();

        return new PagedResultDto<LicenseDto>
        {
            Items = items,
            TotalItems = totalItems,
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber
        };
    }

    public async Task<LicenseDto?> GetLicenseDtoByIdAsync(int licenseId)
    {
        return await _context.Licenses
        .AsNoTracking()
        .Where(l => l.LicenseID == licenseId)
        .Select(l => new LicenseDto(
            l.LicenseID,
            $"{l.Driver.Person.FirstName} {l.Driver.Person.LastName}",
            l.Driver.Person.NationalNo,
            l.Driver.Person.Phone,
            l.LicenseClass.ClassName,
            l.LicenseIssueReason.IssueReasonName,
            l.IssueDate,
            l.ExpirationDate,
            l.IsActive
        ))
        .SingleOrDefaultAsync();
    }

    private async Task<ServiceResult<LocalDrivingLicenseApplication>> ValidateAndGetLocalDrivingApp(int localDrivingAppId)
    {
        LocalDrivingLicenseApplication? localDrivingApp = await _context.LocalDrivingLicenseApplications
        .Include(localApp => localApp.BaseApplication)
        .ThenInclude(baseApp => baseApp.ApplicantPerson)
        .Include(localApp => localApp.LicenseClass)
        .Include(localApp => localApp.BaseApplication)
        .ThenInclude(baseApp => baseApp.License)
        .SingleOrDefaultAsync(localApp => localApp.LocalDrivingLicenseApplicationID == localDrivingAppId);

        if (localDrivingApp == null)
            return ServiceResult<LocalDrivingLicenseApplication>.Failure(["This license application doesn't exist"], FailureType.NotFound);

        if (localDrivingApp.BaseApplication.ApplicationStatusID != (int)enApplicationStatus.New)
            return ServiceResult<LocalDrivingLicenseApplication>.Failure(["This license application is inactive"], FailureType.Conflict);

        if (localDrivingApp.BaseApplication.License != null)
            return ServiceResult<LocalDrivingLicenseApplication>.Failure(["This license application already has a license issued for it"], FailureType.Conflict);

        if (!await _testService.PassedAllRequiredTestsAsync(localDrivingAppId))
            return ServiceResult<LocalDrivingLicenseApplication>.Failure(["This person didn't pass all of the required tests in order to be eligible for this license"], FailureType.Conflict);

        return ServiceResult<LocalDrivingLicenseApplication>.Success(localDrivingApp);
    }

    private License BuildLicenseEntity(LocalDrivingLicenseApplication localDrivingApp, Driver driver, CreateLicenseDto createLicenseDto)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        return new License
        {
            ApplicationID = localDrivingApp.ApplicationID,
            DriverID = driver.DriverID,
            LicenseClassID = localDrivingApp.LicenseClassID,
            IssueDate = today,
            ExpirationDate = today.AddYears(localDrivingApp.LicenseClass.DefaultValidityLength),
            Notes = createLicenseDto.Notes,
            PaidFees = localDrivingApp.LicenseClass.ClassFees,
            IsActive = true,
            IssueReasonID = (int)enLicenseIssueReason.FirstTimeIssue,
            CreatedByUserID = _currentUserService.UserID
        };
    }

    public async Task<ServiceResult<LicenseDto>> CreateNewLicenseAsync(CreateLicenseDto createLicenseDto)
    {
        ServiceResult<LocalDrivingLicenseApplication> localDrivingAppValidation = await ValidateAndGetLocalDrivingApp(createLicenseDto.LocalDrivingLicenseApplicationID);

        if (!localDrivingAppValidation.IsSuccess)
            return ServiceResult<LicenseDto>.Failure(localDrivingAppValidation.Errors, localDrivingAppValidation.ResultType!.Value);

        LocalDrivingLicenseApplication localDrivingApp = localDrivingAppValidation.Data!;
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var (driver, isNew) = await _driverService.GetOrCreateDriverAsync(localDrivingApp.BaseApplication.ApplicantPersonID);

            if (isNew)
            {
                await _context.Drivers.AddAsync(driver);
                await _context.SaveChangesAsync();
            }

            License newLicense = BuildLicenseEntity(localDrivingApp, driver, createLicenseDto);

            await _context.Licenses.AddAsync(newLicense);
            localDrivingApp.BaseApplication.ApplicationStatusID = (int)enApplicationStatus.Completed;
            localDrivingApp.BaseApplication.LastStatusDate = today;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            Application baseApplication = localDrivingApp.BaseApplication;
            Person person = baseApplication.ApplicantPerson;
            LicenseClass licenseClass = localDrivingApp.LicenseClass;

            LicenseDto newLicenseDto = new LicenseDto(
                newLicense.LicenseID,
                $"{person.FirstName} {person.LastName}",
                person.NationalNo,
                person.Phone,
                licenseClass.ClassName,
                ((enLicenseIssueReason)newLicense.IssueReasonID).GetDisplayName(),
                newLicense.IssueDate,
                newLicense.ExpirationDate,
                newLicense.IsActive
            );

            return ServiceResult<LicenseDto>.Success(newLicenseDto);
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<License?> GetLicenseWithDetailsByIdAsync(int licenseId)
    {
        return await _context.Licenses
        .Include(l => l.Driver)
        .ThenInclude(d => d.Person)
        .Include(l => l.LicenseClass)
        .Include(l => l.Detentions)
        .SingleOrDefaultAsync(l => l.LicenseID == licenseId);
    }

    public async Task<License?> GetLicenseByIdForDetentionAsync(int licenseId)
    {
        return await _context.Licenses
        .Include(l => l.Driver)
        .ThenInclude(d => d.Person)
        .Include(l => l.Detentions)
        .SingleOrDefaultAsync(l => l.LicenseID == licenseId);
    }

    public async Task<List<LicenseListItemDto>> GetAllLicensesForDriverAsync(int driverId)
    {
        return await _context.Licenses
        .AsNoTracking()
        .Where(l => l.DriverID == driverId)
        .OrderByDescending(l => l.IssueDate)
        .Select(l => new LicenseListItemDto(
            l.LicenseID,
            l.LicenseClass.ClassName,
            l.LicenseIssueReason.IssueReasonName,
            l.IssueDate,
            l.ExpirationDate,
            l.IsActive
        )).ToListAsync();
    }
}