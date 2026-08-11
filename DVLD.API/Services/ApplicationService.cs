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
using DVLD.API.DTOs;
using DVLD.API.DTOs.DetainedLicenses;

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
    private readonly IInternationalLicenseService _internationalLicenseService;

    public ApplicationService(DVLDContext context, IApplicationTypeService applicationTypeService, ILicenseClassService licenseClassService, IPersonService personService, ICurrentUserService currentUserService, IDetainedLicenseService detainedLicenseService, ITestService testService, ITestAppointmentService testAppointmentService, ILicenseService licenseService, IInternationalLicenseService internationalLicenseService)
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
        _internationalLicenseService = internationalLicenseService;
    }

    public async Task<PagedResultDto<ApplicationDto>> GetAllApplicationsAsync(ApplicationsQueryParameters parameters)
    {
        IQueryable<Application> query = _context.Applications
        .AsNoTracking()
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

    public async Task<ApplicationDto?> GetApplicationDtoByIdAsync(int applicationId)
    {
        return await _context.Applications
        .AsNoTracking()
        .Where(app => app.ApplicationID == applicationId)
        .Select(app => new ApplicationDto(
            app.ApplicationID,
            $"{app.ApplicantPerson.FirstName} {app.ApplicantPerson.LastName}",
            app.ApplicantPerson.NationalNo,
            app.ApplicationDate,
            app.ApplicationType.ApplicationTypeTitle,
            app.ApplicationStatus.StatusName,
            app.CreatedByUser.UserName
        )).SingleOrDefaultAsync();
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

    private Application BuildApplicationEntity(int applicantPersonId, enApplicationType applicationType, enApplicationStatus applicationStatus, decimal appFees)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        return new Application
        {
            ApplicantPersonID = applicantPersonId,
            ApplicationDate = today,
            ApplicationTypeID = (int)applicationType,
            ApplicationStatusID = (int)applicationStatus,
            LastStatusDate = today,
            PaidFees = appFees,
            CreatedByUserID = _currentUserService.UserID
        };
    }

    private ApplicationDto BuildApplicationDto(Application newApplication, Person person, enApplicationType appType)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        return new ApplicationDto(
            newApplication.ApplicationID,
            $"{person.FirstName} {person.LastName}",
            person.NationalNo,
            today,
            appType.GetDisplayName(),
            enApplicationStatus.New.GetDisplayName(),
            _currentUserService.UserName
        );
    }

    public async Task<ServiceResult<ApplicationDto>> CreateLocalDrivingLicenseApplicationAsync(CreateLocalDrivingLicenseApplicationDto createLocalDrivingAppDto)
    {
        ServiceResult<Person> referenceDataForNewLicenseValidation = await ValidateReferenceDataForNewLicenseAsync(createLocalDrivingAppDto);

        if (!referenceDataForNewLicenseValidation.IsSuccess)
            return ServiceResult<ApplicationDto>.Failure(referenceDataForNewLicenseValidation.Errors, referenceDataForNewLicenseValidation.ResultType!.Value);

        Person person = referenceDataForNewLicenseValidation.Data!;
        decimal appFees = await _applicationTypeService.GetApplicationTypeFeesAsync(enApplicationType.NewLocalDrivingLicense);

        Application newApplication = BuildApplicationEntity(createLocalDrivingAppDto.ApplicantPersonID, enApplicationType.NewLocalDrivingLicense, enApplicationStatus.New, appFees);

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

        if (await _testAppointmentService.RetakeTestAlreadyBookedAsync(failedTest.TestAppointment.LocalDrivingLicenseApplicationID, failedTest.TestAppointment.TestTypeID))
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
            CreatedByUserID = _currentUserService.UserID,
            IsLocked = false,
            RetakeTestApplicationID = retakeApplicationId
        };
    }

    public async Task<ServiceResult<TestAppointmentDto>> CreateRetakeTestApplicationAsync(CreateRetakeTestApplicationDto createRetakeTestApplicationDto)
    {
        ServiceResult<Test> validationForNewTest = await ValidateForTestEligibility(createRetakeTestApplicationDto.TestID);

        if (!validationForNewTest.IsSuccess)
            return ServiceResult<TestAppointmentDto>.Failure(validationForNewTest.Errors, validationForNewTest.ResultType!.Value);

        Test failedTest = validationForNewTest.Data!;
        int applicantPersonId = failedTest.TestAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.PersonID;
        decimal appFees = await _applicationTypeService.GetApplicationTypeFeesAsync(enApplicationType.RetakeTest);

        Application newRetakeApplication = BuildApplicationEntity(applicantPersonId, enApplicationType.RetakeTest, enApplicationStatus.New, appFees);

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
                _currentUserService.UserName
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

        if (!oldLicense.IsActive)
            return ServiceResult<License>.Failure(["Cannot renew an inactive license"], FailureType.Conflict);

        if (oldLicense.ExpirationDate > today.AddDays(30))
            return ServiceResult<License>.Failure(["License is not yet eligible for renewal"], FailureType.Conflict);

        if (oldLicense.Detentions.Any(d => d.ReleaseDate == null))
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
            CreatedByUserID = _currentUserService.UserID
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

        Application renewLicenseApplication = BuildApplicationEntity(applicantPersonId, enApplicationType.RenewDrivingLicense, enApplicationStatus.Completed, appFees);

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

    private ServiceResult<License> GeneralLicenseValidation(License license)
    {
        if (!license.IsActive)
            return ServiceResult<License>.Failure(["Cannot replace an inactive license"], FailureType.Conflict);

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        if (license.ExpirationDate < today)
            return ServiceResult<License>.Failure(["Expired licenses must be renewed instead of replaced"], FailureType.Conflict);

        if (license.Detentions.Any(d => d.ReleaseDate == null))
            return ServiceResult<License>.Failure(["A detained license cannot be replaced before it is released"], FailureType.Conflict);

        return ServiceResult<License>.Success(license);
    }

    private async Task<ServiceResult<License>> ValidateLicenseForReplacement(ReplaceLicenseDto replaceLicenseDto)
    {
        if (replaceLicenseDto.ReplacementReason != ReplaceLicenseDto.enReplacementReason.ReplaceDamagedLicense
        && replaceLicenseDto.ReplacementReason != ReplaceLicenseDto.enReplacementReason.ReplaceLostLicense)
            return ServiceResult<License>.Failure(["Replacement reason is invalid"], FailureType.ValidationError);

        License? license = await _licenseService.GetLicenseWithDetailsByIdAsync(replaceLicenseDto.LicenseID);

        if (license == null)
            return ServiceResult<License>.Failure(["This license does not exist"], FailureType.NotFound);

        return GeneralLicenseValidation(license);
    }

    public async Task<ServiceResult<LicenseDto>> ReplaceDrivingLicenseAsync(ReplaceLicenseDto replaceLicenseDto)
    {
        ServiceResult<License> replacementValidation = await ValidateLicenseForReplacement(replaceLicenseDto);

        if (!replacementValidation.IsSuccess)
            return ServiceResult<LicenseDto>.Failure(replacementValidation.Errors, replacementValidation.ResultType!.Value);

        License license = replacementValidation.Data!;
        int applicantPersonId = license.Driver.PersonID;
        enApplicationType applicationType = replaceLicenseDto.ReplacementReason == ReplaceLicenseDto.enReplacementReason.ReplaceLostLicense ? enApplicationType.ReplacementForLostDrivingLicense : enApplicationType.ReplacementForDamagedDrivingLicense;
        decimal appFees = await _applicationTypeService.GetApplicationTypeFeesAsync(applicationType);

        Application replaceLicenseApplication = BuildApplicationEntity(applicantPersonId, applicationType, enApplicationStatus.Completed, appFees);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Applications.AddAsync(replaceLicenseApplication);
            await _context.SaveChangesAsync();

            Driver driver = license.Driver;
            Person person = driver.Person;
            LicenseClass licenseClass = license.LicenseClass;
            enLicenseIssueReason licenseIssueReason = replaceLicenseDto.ReplacementReason == ReplaceLicenseDto.enReplacementReason.ReplaceLostLicense ? enLicenseIssueReason.ReplacementForLostLicense : enLicenseIssueReason.ReplacementForDamagedLicense;

            License replacedLicense = BuildLicenseEntity(replaceLicenseApplication, license, license.IssueDate, license.ExpirationDate, replaceLicenseDto.Notes, licenseIssueReason);
            license.IsActive = false;

            await _context.Licenses.AddAsync(replacedLicense);
            await _context.SaveChangesAsync();

            LicenseDto newLicenseDto = new LicenseDto(
                replacedLicense.LicenseID,
                $"{person.FirstName} {person.LastName}",
                person.NationalNo,
                person.Phone,
                licenseClass.ClassName,
                licenseIssueReason.GetDisplayName(),
                replacedLicense.IssueDate,
                replacedLicense.ExpirationDate,
                replacedLicense.IsActive
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

    private async Task<ServiceResult<License>> ValidateLicenseForInternationalLicenseIssuance(int licenseId)
    {
        License? license = await _licenseService.GetLicenseWithDetailsByIdAsync(licenseId);

        if (license == null)
            return ServiceResult<License>.Failure(["This license does not exist"], FailureType.NotFound);

        if (await _internationalLicenseService.ActiveInternationalLicenseExistsAsync(license.DriverID))
            return ServiceResult<License>.Failure(["This driver already has an active international license"], FailureType.Conflict);

        return GeneralLicenseValidation(license);
    }

    private InternationalLicense BuildInternationalLicenseEntity(Application intlLicenseApplication, License license, IssueInternationalLicenseDto issueInternationalLicenseDto)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        return new InternationalLicense
        {
            ApplicationID = intlLicenseApplication.ApplicationID,
            DriverID = license.DriverID,
            IssuedUsingLocalLicenseID = license.LicenseID,
            IssueDate = today,
            ExpirationDate = today.AddYears(1),
            IsActive = true,
            CreatedByUserID = _currentUserService.UserID
        };
    }

    public async Task<ServiceResult<InternationalLicenseDto>> IssueInternationalLicenseAsync(IssueInternationalLicenseDto issueInternationalLicenseDto)
    {
        ServiceResult<License> internationalLicenseIssuanceValidation = await ValidateLicenseForInternationalLicenseIssuance(issueInternationalLicenseDto.LicenseID);

        if (!internationalLicenseIssuanceValidation.IsSuccess)
            return ServiceResult<InternationalLicenseDto>.Failure(internationalLicenseIssuanceValidation.Errors, internationalLicenseIssuanceValidation.ResultType!.Value);

        License license = internationalLicenseIssuanceValidation.Data!;
        int applicantPersonId = license.Driver.PersonID;
        decimal appFees = await _applicationTypeService.GetApplicationTypeFeesAsync(enApplicationType.NewInternationalLicense);

        Application internationalLicenseApplication = BuildApplicationEntity(applicantPersonId, enApplicationType.NewInternationalLicense, enApplicationStatus.Completed, appFees);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Applications.AddAsync(internationalLicenseApplication);
            await _context.SaveChangesAsync();

            Driver driver = license.Driver;
            Person person = driver.Person;
            LicenseClass licenseClass = license.LicenseClass;

            InternationalLicense internationalLicense = BuildInternationalLicenseEntity(internationalLicenseApplication, license, issueInternationalLicenseDto);

            await _context.InternationalLicenses.AddAsync(internationalLicense);
            await _context.SaveChangesAsync();

            InternationalLicenseDto newInternationalLicenseDto = new InternationalLicenseDto(
                internationalLicense.InternationalLicenseID,
                $"{person.FirstName} {person.LastName}",
                person.NationalNo,
                person.Phone,
                licenseClass.ClassName,
                internationalLicense.IssueDate,
                internationalLicense.ExpirationDate,
                internationalLicense.IsActive
            );

            await transaction.CommitAsync();

            return ServiceResult<InternationalLicenseDto>.Success(newInternationalLicenseDto);
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<ServiceResult<(License license, DetainedLicense detainInfo)>> ValidateAndGetLicenseForRelease(int licenseId)
    {
        License? license = await _licenseService.GetLicenseByIdForDetentionAsync(licenseId);

        if (license == null)
            return ServiceResult<(License license, DetainedLicense detainInfo)>.Failure(["This license does not exist"], FailureType.NotFound);

        if (license.IsActive)
            return ServiceResult<(License license, DetainedLicense detainInfo)>.Failure(["This license is active"], FailureType.Conflict);

        DetainedLicense? detainInfo = license.Detentions.SingleOrDefault(d => d.ReleaseDate == null);

        if (detainInfo == null)
            return ServiceResult<(License license, DetainedLicense detainInfo)>.Failure(["This license is not currently detained"], FailureType.Conflict);

        return ServiceResult<(License license, DetainedLicense detainInfo)>.Success((license, detainInfo));
    }

    public async Task<ServiceResult<DetainedLicenseDto>> ReleaseDetainedLicenseAsync(ReleaseDetainedLicenseDto releaseDetainedLicenseDto)
    {
        ServiceResult<(License license, DetainedLicense detainInfo)> licenseValidationBeforeRelease = await ValidateAndGetLicenseForRelease(releaseDetainedLicenseDto.LicenseID);

        if (!licenseValidationBeforeRelease.IsSuccess)
            return ServiceResult<DetainedLicenseDto>.Failure(licenseValidationBeforeRelease.Errors, licenseValidationBeforeRelease.ResultType!.Value);

        License license = licenseValidationBeforeRelease.Data.license;
        DetainedLicense detainInfo = licenseValidationBeforeRelease.Data.detainInfo;
        Driver driver = license.Driver;
        Person person = driver.Person;
        decimal appFees = await _applicationTypeService.GetApplicationTypeFeesAsync(enApplicationType.ReleaseDetainedDrivingLicense);

        Application releaseDetainedLicenseApplication = BuildApplicationEntity(person.PersonID, enApplicationType.ReleaseDetainedDrivingLicense, enApplicationStatus.Completed, appFees);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Applications.AddAsync(releaseDetainedLicenseApplication);
            await _context.SaveChangesAsync();

            license.IsActive = true;
            detainInfo.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            detainInfo.ReleasedByUserID = _currentUserService.UserID;
            detainInfo.ReleaseApplicationID = releaseDetainedLicenseApplication.ApplicationID;

            await _context.SaveChangesAsync();

            DetainedLicenseDto newDetainInfoDto = new DetainedLicenseDto(
                detainInfo.DetainID,
                $"{person.FirstName} {person.LastName}",
                person.NationalNo,
                person.Phone,
                detainInfo.FineFees,
                detainInfo.DetainDate,
                detainInfo.ReleaseDate,
                _currentUserService.UserName
            );

            await transaction.CommitAsync();

            return ServiceResult<DetainedLicenseDto>.Success(newDetainInfoDto);
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

