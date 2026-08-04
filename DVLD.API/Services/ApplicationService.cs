using DVLD.API.Common.QueryParameters;
using DVLD.API.DTOs.Applications;
using DVLD.API.DTOs.Common;
using DVLD.API.Extensions;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.API.Common.Results;
using DVLD.API.Common.Constants;
using Microsoft.EntityFrameworkCore;
using DVLD.API.Mappings.Applications;
using DVLD.API.DTOs.TestAppointments;

namespace DVLD.API.Services;

public class ApplicationService : IApplicationService
{
    private readonly DVLDContext _context;
    private readonly IPersonService _personService;
    private readonly IApplicationTypeService _applicationTypeService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalDrivingLicenseApplicationService _localDrivingLicenseApplicationService;
    private readonly ITestService _testService;
    private readonly ITestAppointmentService _testAppointmentService;

    public ApplicationService(DVLDContext context, IApplicationTypeService applicationTypeService, IApplicationStatusService applicationStatusService, IPersonService personService, ICurrentUserService currentUserService, ILocalDrivingLicenseApplicationService localDrivingLicenseApplicationService, ITestService testService, ITestAppointmentService testAppointmentService)
    {
        _context = context;
        _applicationTypeService = applicationTypeService;
        _personService = personService;
        _currentUserService = currentUserService;
        _localDrivingLicenseApplicationService = localDrivingLicenseApplicationService;
        _testService = testService;
        _testAppointmentService = testAppointmentService;
    }

    public async Task<PagedResultDto<ApplicationDto>> GetAllApplicationsAsync(ApplicationsQueryParameters parameters)
    {
        IQueryable<Application> query = _context.Applications.AsNoTracking()
        .Include(app => app.ApplicantPerson)
        .Include(app => app.CreatedByUser)
        .Include(app => app.ApplicationType)
        .Include(app => app.ApplicationStatus)
        .ApplySearch(parameters.SearchTerm)
        .ApplyFilters(parameters)
        .ApplySort(parameters);

        int totalItems = await query.CountAsync();

        List<ApplicationDto> items = await query.ApplyPagination(parameters)
        .Select(app => new ApplicationDto(
            app.ApplicationID,
            $"{app.ApplicantPerson.FirstName} {app.ApplicantPerson.LastName}",
            app.ApplicantPerson.NationalNo,
            app.ApplicationDate,
            app.ApplicationType.ApplicationTypeTitle,
            app.ApplicationStatus.StatusName,
            app.CreatedByUser.UserName
        )).ToListAsync();

        return new PagedResultDto<ApplicationDto>
        {
            Items = items,
            TotalItems = totalItems,
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber
        };
    }

    private async Task<ServiceResult<Application>> BuildApplicationEntityAsync(int applicantPersonId, int applicationTypeId)
    {
        ApplicationType? appType = await _applicationTypeService.GetApplicationTypeByIdAsync(applicationTypeId);

        if (appType == null)
            return ServiceResult<Application>.Failure(["This application type does not exist"], FailureType.Conflict);

        if (!await _personService.PersonExistsAsync(applicantPersonId))
            return ServiceResult<Application>.Failure(["This person does not exist"], FailureType.Conflict);

        int? createdByUserID = _currentUserService.UserID;

        if (createdByUserID == null)
            return ServiceResult<Application>.Failure(["Unable to determine current user"], FailureType.Unauthorized);

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        Application application = new Application
        {
            ApplicantPersonID = applicantPersonId,
            ApplicationDate = today,
            ApplicationTypeID = appType.ApplicationTypeID,
            ApplicationStatusID = (int)enApplicationStatus.New,
            LastStatusDate = today,
            PaidFees = appType.ApplicationFees,
            CreatedByUserID = createdByUserID.Value
        };

        return ServiceResult<Application>.Success(application);
    }

    public async Task<ServiceResult<ApplicationDto>> CreateNewDrivingLicenseApplicationAsync(CreateLocalDrivingLicenseApplicationDto dto)
    {
        ServiceResult<Application> baseAppEntityCreationResult = await BuildApplicationEntityAsync(dto.ApplicantPersonID, (int)enApplicationType.NewLocalDrivingLicense);

        if (!baseAppEntityCreationResult.IsSuccess)
            return ServiceResult<ApplicationDto>.Failure(baseAppEntityCreationResult.Errors, baseAppEntityCreationResult.ResultType!.Value);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            Application application = baseAppEntityCreationResult.Data!;
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();

            ServiceResult<LocalDrivingLicenseApplication> licenseAppResult =
            await _localDrivingLicenseApplicationService.BuildLocalDrivingLicenseEntityAsync(application.ApplicationID, dto.LicenseClassID);

            if (!licenseAppResult.IsSuccess)
            {
                await transaction.RollbackAsync();
                return ServiceResult<ApplicationDto>.Failure(licenseAppResult.Errors, licenseAppResult.ResultType!.Value);
            }

            await _context.LocalDrivingLicenseApplications.AddAsync(licenseAppResult.Data!);

            await _context.SaveChangesAsync();

            Application savedApplication = await _context.Applications
            .Where(app => app.ApplicationID == application.ApplicationID)
            .AsNoTracking()
            .Include(app => app.ApplicantPerson)
            .Include(app => app.CreatedByUser)
            .Include(app => app.ApplicationType)
            .Include(app => app.ApplicationStatus)
            .SingleAsync();

            await transaction.CommitAsync();

            return ServiceResult<ApplicationDto>.Success(savedApplication.ToDto());
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ServiceResult<TestAppointmentDto>> CreateNewRetakeTestApplication(CreateRetakeTestApplicationDto dto)
    {
        Test? failedTest = await _testService.GetTestByTestIdAsync(dto.TestID);

        if (failedTest == null)
            return ServiceResult<TestAppointmentDto>.Failure(["This failed test doesn't exist"], FailureType.Conflict);

        if (failedTest.Passed)
            return ServiceResult<TestAppointmentDto>.Failure(["This applicant has passed this test"], FailureType.Conflict);

        if (await _testAppointmentService.RetakeTestAlreadyBooked(failedTest.TestAppointment.LocalDrivingLicenseApplicationID, failedTest.TestAppointment.TestTypeID))
            return ServiceResult<TestAppointmentDto>.Failure(["This applicant already has a retake appointment for this test"], FailureType.Conflict);

        int? createdByUserID = _currentUserService.UserID;
        string? createdByUserName = _currentUserService.UserName;

        if (createdByUserID == null || createdByUserName == null)
            return ServiceResult<TestAppointmentDto>.Failure(["Unable to determine current user"], FailureType.Unauthorized);

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            Application retakeApplication = new Application
            {
                ApplicantPersonID = failedTest.TestAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicantPersonID,
                ApplicationDate = today,
                ApplicationTypeID = (int)enApplicationType.RetakeTest,
                ApplicationStatusID = (int)enApplicationStatus.New,
                LastStatusDate = today,
                PaidFees = failedTest.TestAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicationType.ApplicationFees,
                CreatedByUserID = createdByUserID.Value
            };

            await _context.Applications.AddAsync(retakeApplication);
            await _context.SaveChangesAsync();

            TestAppointment retakeTestAppointment = new TestAppointment
            {
                TestTypeID = failedTest.TestAppointment.TestType.TestTypeID,
                LocalDrivingLicenseApplicationID = failedTest.TestAppointment.LocalDrivingLicenseApplicationID,
                AppointmentTime = dto.AppointmentTime,
                PaidFees = failedTest.TestAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicationType.ApplicationFees,
                CreatedByUserID = createdByUserID.Value,
                IsLocked = false,
                RetakeTestApplicationID = retakeApplication.ApplicationID
            };

            await _context.TestAppointments.AddAsync(retakeTestAppointment);
            await _context.SaveChangesAsync();

            TestAppointmentDto resultDto = new TestAppointmentDto(
                retakeTestAppointment.TestAppointmentID,
                failedTest.TestAppointment.TestType.TestTypeTitle,
                $"{failedTest.TestAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.FirstName} {failedTest.TestAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.LastName}",
                failedTest.TestAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.NationalNo,
                dto.AppointmentTime,
                createdByUserName
            );

            await transaction.CommitAsync();

            return ServiceResult<TestAppointmentDto>.Success(resultDto);
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

    }

}