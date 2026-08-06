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
using DVLD.API.DTOs.TestAppointments;
using Microsoft.OpenApi.Extensions;
using DVLD.API.DTOs.Licenses;

namespace DVLD.API.Services;

public class ApplicationService : IApplicationService
{
    private readonly DVLDContext _context;
    private readonly IPersonService _personService;
    private readonly IApplicationTypeService _applicationTypeService;
    private readonly ILicenseClassService _licenseClassService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDetainedLicenseService _detainedLicenseService;
    private readonly ITestService _testService;
    private readonly ITestAppointmentService _testAppointmentService;
    private readonly ILicenseService _licenseService;

    public ApplicationService(DVLDContext context, IApplicationTypeService applicationTypeService, ILicenseClassService licenseClassService, IPersonService personService, ICurrentUserService currentUserService, IDetainedLicenseService detainedLicenseService, ITestService testService, ITestAppointmentService testAppointmentService, ILicenseService licenseService)
    {
        _context = context;
        _applicationTypeService = applicationTypeService;
        _personService = personService;
        _licenseClassService = licenseClassService;
        _currentUserService = currentUserService;
        _testService = testService;
        _testAppointmentService = testAppointmentService;
        _licenseService = licenseService;
        _detainedLicenseService = detainedLicenseService;
    }

    public async Task<PagedResultDto<ApplicationDto>> GetAllApplicationsAsync(ApplicationsQueryParameters parameters)
    {
        IQueryable<Application> query = _context.Applications
        .AsNoTracking()
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

    private async Task<ServiceResult<Person>> ValidateReferenceDataForNewLicenseAsync(CreateLocalDrivingLicenseApplicationDto createLocalDrivingAppDto)
    {
        Person? person = await _personService.GetPersonByIdAsync(createLocalDrivingAppDto.ApplicantPersonID);

        if (person == null)
            return ServiceResult<Person>.Failure(["This person does not exist"], FailureType.NotFound);

        if (!await _licenseClassService.LicenseClassExistsAsync(createLocalDrivingAppDto.LicenseClassID))
            return ServiceResult<Person>.Failure(["This license class does not exist"], FailureType.NotFound);

        return ServiceResult<Person>.Success(person);
    }

    private ServiceResult ValidateCurrentUser()
    {
        if (_currentUserService.UserID == null || _currentUserService.UserName == null)
            return ServiceResult.Failure(["Unable to determine current user"], FailureType.Unauthorized);

        return ServiceResult.Success();
    }

    private async Task<ServiceResult<Application>> BuildApplicationEntityAsync(int applicantPersonId, enApplicationType applicationType, enApplicationStatus applicationStatus, decimal appFees)
    {
        ServiceResult currentUserValidation = ValidateCurrentUser();

        if (!currentUserValidation.IsSuccess)
            return ServiceResult<Application>.Failure(currentUserValidation.Errors, currentUserValidation.ResultType!.Value);

        int currentUserId = _currentUserService.UserID!.Value;

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        Application application = new Application
        {
            ApplicantPersonID = applicantPersonId,
            ApplicationDate = today,
            ApplicationTypeID = (int)applicationType,
            ApplicationStatusID = (int)enApplicationStatus.New,
            LastStatusDate = today,
            PaidFees = appFees,
            CreatedByUserID = currentUserId
        };

        return ServiceResult<Application>.Success(application);
    }

    private ApplicationDto BuildApplicationDto(Application newApplication, Person person, enApplicationType appType)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        string currentUserName = _currentUserService.UserName!;

        return new ApplicationDto(
            newApplication.ApplicationID,
            $"{person.FirstName} {person.LastName}",
            person.NationalNo,
            today,
            appType.GetDisplayName(),
            enApplicationStatus.New.GetDisplayName(),
            currentUserName
        );
    }

    public async Task<ServiceResult<ApplicationDto>> CreateNewDrivingLicenseApplicationAsync(CreateLocalDrivingLicenseApplicationDto createLocalDrivingAppDto)
    {
        ServiceResult<Person> referenceDataForNewLicenseValidation = await ValidateReferenceDataForNewLicenseAsync(createLocalDrivingAppDto);

        if (!referenceDataForNewLicenseValidation.IsSuccess)
            return ServiceResult<ApplicationDto>.Failure(referenceDataForNewLicenseValidation.Errors, referenceDataForNewLicenseValidation.ResultType!.Value);

        Person person = referenceDataForNewLicenseValidation.Data!;
        decimal appFees = await _applicationTypeService.GetApplicationTypeFeesAsync(enApplicationType.NewLocalDrivingLicense);

        ServiceResult<Application> newApplicationValidation = await BuildApplicationEntityAsync(createLocalDrivingAppDto.ApplicantPersonID, enApplicationType.NewLocalDrivingLicense, enApplicationStatus.New, appFees);

        if (!newApplicationValidation.IsSuccess)
            return ServiceResult<ApplicationDto>.Failure(newApplicationValidation.Errors, newApplicationValidation.ResultType!.Value);

        Application newApplication = newApplicationValidation.Data!;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Applications.AddAsync(newApplication);
            await _context.SaveChangesAsync();

            LocalDrivingLicenseApplication localDrivingApp = new LocalDrivingLicenseApplication
            {
                ApplicationID = newApplication.ApplicationID,
                LicenseClassID = createLocalDrivingAppDto.LicenseClassID
            };

            await _context.LocalDrivingLicenseApplications.AddAsync(localDrivingApp);

            await _context.SaveChangesAsync();

            ApplicationDto applicationDto = BuildApplicationDto(newApplication, person, enApplicationType.NewLocalDrivingLicense);
            await transaction.CommitAsync();

            return ServiceResult<ApplicationDto>.Success(applicationDto);
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<ServiceResult<Test>> ValidateForTestEligibility(int testId)
    {
        Test? failedTest = await _testService.GetTestWithDetailsByTestIdAsync(testId);

        if (failedTest == null)
            return ServiceResult<Test>.Failure(["This failed test doesn't exist"], FailureType.NotFound);

        if (failedTest.Passed)
            return ServiceResult<Test>.Failure(["This applicant has passed this test"], FailureType.Conflict);

        if (await _testAppointmentService.RetakeTestAlreadyBooked(failedTest.TestAppointment.LocalDrivingLicenseApplicationID, failedTest.TestAppointment.TestTypeID))
            return ServiceResult<Test>.Failure(["This applicant already has a retake appointment for this test"], FailureType.Conflict);

        return ServiceResult<Test>.Success(failedTest);
    }

    private TestAppointment BuildRetakeAppointmentEntity(
        int testTypeId,
        int localDrivingApplicationId,
        decimal testFee,
        DateTime appointmentTime,
        int retakeApplicationId)
    {
        return new TestAppointment
        {
            TestTypeID = testTypeId,
            LocalDrivingLicenseApplicationID = localDrivingApplicationId,
            AppointmentTime = appointmentTime,
            PaidFees = testFee,
            CreatedByUserID = _currentUserService.UserID!.Value,
            IsLocked = false,
            RetakeTestApplicationID = retakeApplicationId
        };
    }

    public async Task<ServiceResult<TestAppointmentDto>> CreateNewRetakeTestApplication(CreateRetakeTestApplicationDto createRetakeTestApplicationDto)
    {
        ServiceResult<Test> validationForNewTest = await ValidateForTestEligibility(createRetakeTestApplicationDto.TestID);

        if (!validationForNewTest.IsSuccess)
            return ServiceResult<TestAppointmentDto>.Failure(validationForNewTest.Errors, validationForNewTest.ResultType!.Value);

        Test failedTest = validationForNewTest.Data!;
        int applicantPersonId = failedTest.TestAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.PersonID;
        decimal appFees = await _applicationTypeService.GetApplicationTypeFeesAsync(enApplicationType.RetakeTest);

        ServiceResult<Application> newApplicationValidation = await BuildApplicationEntityAsync(applicantPersonId, enApplicationType.RetakeTest, enApplicationStatus.New, appFees);

        if (!newApplicationValidation.IsSuccess)
            return ServiceResult<TestAppointmentDto>.Failure(newApplicationValidation.Errors, newApplicationValidation.ResultType!.Value);

        Application newRetakeApplication = newApplicationValidation.Data!;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Applications.AddAsync(newRetakeApplication);
            await _context.SaveChangesAsync();

            TestAppointment failedAppointment = failedTest.TestAppointment;
            LocalDrivingLicenseApplication localApp = failedAppointment.LocalDrivingLicenseApplication;
            Application application = localApp.BaseApplication;
            Person applicant = application.ApplicantPerson;
            TestType testType = failedAppointment.TestType;

            TestAppointment retakeTestAppointment = BuildRetakeAppointmentEntity(failedAppointment.TestTypeID, localApp.LocalDrivingLicenseApplicationID, testType.TestTypeFees, createRetakeTestApplicationDto.AppointmentTime, newRetakeApplication.ApplicationID);

            await _context.TestAppointments.AddAsync(retakeTestAppointment);
            await _context.SaveChangesAsync();

            TestAppointmentDto resultDto = new TestAppointmentDto(
                retakeTestAppointment.TestAppointmentID,
                testType.TestTypeTitle,
                $"{applicant.FirstName} {applicant.LastName}",
                applicant.NationalNo,
                createRetakeTestApplicationDto.AppointmentTime,
                _currentUserService.UserName!
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

    private async Task<ServiceResult<License>> ValidateLicenseForRenewal(int licenseId)
    {
        License? oldLicense = await _licenseService.GetLicenseWithDetailsByIdAsync(licenseId);
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        if (oldLicense == null)
            return ServiceResult<License>.Failure(["This license does not exist"], FailureType.NotFound);

        if (oldLicense.IsActive)
            return ServiceResult<License>.Failure(["Cannot replace an active license"], FailureType.Conflict);

        if (oldLicense.ExpirationDate > today.AddDays(30))
            return ServiceResult<License>.Failure(["License is not yet eligible for renewal"], FailureType.Conflict);

        if (await _detainedLicenseService.IsLicenseDetainedAsync(licenseId))
            return ServiceResult<License>.Failure(["A detained license cannot be renewed before it is released"], FailureType.Conflict);

        return ServiceResult<License>.Success(oldLicense);
    }

    private License BuildLicenseEntity(
        Application application,
        License oldLicense,
        DateOnly issueDate,
        DateOnly expirationDate,
        string? notes,
        enLicenseIssueReason issueReason)
    {
        return new License
        {
            ApplicationID = application.ApplicationID,
            DriverID = oldLicense.DriverID,
            LicenseClassID = oldLicense.LicenseClassID,
            IssueDate = issueDate,
            ExpirationDate = expirationDate,
            Notes = notes,
            PaidFees = oldLicense.LicenseClass.ClassFees,
            IsActive = true,
            IssueReasonID = (int)issueReason,
            CreatedByUserID = _currentUserService.UserID!.Value
        };
    }

    public async Task<ServiceResult<LicenseDto>> RenewDrivingLicenseAsync(RenewLicenseDto renewLicenseDto)
    {
        ServiceResult<License> validationForOldLicense = await ValidateLicenseForRenewal(renewLicenseDto.LicenseID);

        if (!validationForOldLicense.IsSuccess)
            return ServiceResult<LicenseDto>.Failure(validationForOldLicense.Errors, validationForOldLicense.ResultType!.Value);

        License oldLicense = validationForOldLicense.Data!;
        int applicantPersonId = oldLicense.Driver.PersonID;
        decimal appFees = await _applicationTypeService.GetApplicationTypeFeesAsync(enApplicationType.RenewDrivingLicense);

        ServiceResult<Application> newApplicationValidation = await BuildApplicationEntityAsync(applicantPersonId, enApplicationType.RenewDrivingLicense, enApplicationStatus.Completed, appFees);

        if (!newApplicationValidation.IsSuccess)
            return ServiceResult<LicenseDto>.Failure(newApplicationValidation.Errors, newApplicationValidation.ResultType!.Value);

        Application renewLicenseApplication = newApplicationValidation.Data!;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Applications.AddAsync(renewLicenseApplication);
            await _context.SaveChangesAsync();

            Driver driver = oldLicense.Driver;
            Person person = driver.Person;
            LicenseClass licenseClass = oldLicense.LicenseClass;
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            DateOnly expirationDate = today.AddYears(oldLicense.LicenseClass.DefaultValidityLength);

            License renewedLicense = BuildLicenseEntity(renewLicenseApplication, oldLicense, today, expirationDate, renewLicenseDto.Notes, enLicenseIssueReason.Renewal);
            oldLicense.IsActive = false;

            await _context.Licenses.AddAsync(renewedLicense);
            await _context.SaveChangesAsync();

            LicenseDto newLicenseDto = new LicenseDto(
                renewedLicense.LicenseID,
                $"{person.FirstName} {person.LastName}",
                person.NationalNo,
                person.Phone,
                licenseClass.ClassName,
                enLicenseIssueReason.Renewal.GetDisplayName(),
                renewedLicense.IssueDate,
                renewedLicense.ExpirationDate,
                renewedLicense.IsActive
            );

            await transaction.CommitAsync();

            return ServiceResult<LicenseDto>.Success(newLicenseDto);
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}