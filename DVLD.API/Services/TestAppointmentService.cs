using DVLD.API.Common.Constants;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.TestAppointments;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.API.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class TestAppointmentService : ITestAppointmentService
{
    private readonly DVLDContext _context;
    private readonly ITestTypeService _testTypeService;
    private readonly ILocalDrivingLicenseApplicationService _localDrivingLicenseApplicationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITestService _testService;

    public TestAppointmentService(DVLDContext context, ITestTypeService testTypeService, ILocalDrivingLicenseApplicationService localDrivingLicenseApplication, ICurrentUserService currentUserService, ITestService testService)
    {
        _context = context;
        _testTypeService = testTypeService;
        _localDrivingLicenseApplicationService = localDrivingLicenseApplication;
        _currentUserService = currentUserService;
        _testService = testService;
    }

    public async Task<TestAppointmentDto?> GetTestAppointmentDtoByIdAsync(int testAppointmentId)
    {
        return await _context.TestAppointments
        .AsNoTracking()
        .Where(ta => ta.TestAppointmentID == testAppointmentId)
        .Select(ta => new TestAppointmentDto(
            ta.TestAppointmentID,
            ta.TestType.TestTypeTitle,
            $"{ta.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.FirstName} {ta.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.LastName}",
            ta.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.NationalNo,
            ta.AppointmentTime,
            ta.CreatedByUser.UserName
        ))
        .SingleOrDefaultAsync();
    }

    private async Task<ServiceResult<(LocalDrivingLicenseApplication localDrivingApp, TestType testType)>> ValidateTestAppointmentAsync(CreateTestAppointmentDto createTestAppointmentDto)
    {
        TestType? testType = await _testTypeService.GetTestTypeByIdAsync(createTestAppointmentDto.TestTypeID);

        if (testType == null)
            return ServiceResult<(LocalDrivingLicenseApplication localDrivingApp, TestType testType)>.Failure(["This test type does not exist"], FailureType.NotFound);

        int testTypeId = createTestAppointmentDto.TestTypeID;
        string testName = ((enTestType)testTypeId).GetDisplayName();
        string previousTestName = "";

        if (testTypeId != (int)enTestType.VisionTest)
            previousTestName = ((enTestType)testTypeId - 1).GetDisplayName();

        LocalDrivingLicenseApplication? localDrivingApp = await _localDrivingLicenseApplicationService.GetLocalDrivingLicenseApplicationById(createTestAppointmentDto.LocalDrivingLicenseApplicationID);

        if (localDrivingApp == null)
            return ServiceResult<(LocalDrivingLicenseApplication localDrivingApp, TestType testType)>.Failure(["This local driving license application doesn't exist"], FailureType.NotFound);

        if (!_localDrivingLicenseApplicationService.IsActive(localDrivingApp))
            return ServiceResult<(LocalDrivingLicenseApplication localDrivingApp, TestType testType)>.Failure(["Cannot schedule a test appointment because the application is not active"], FailureType.Conflict);

        Test? previousTest = await _testService.GetTestForTestAppointmentAsync(createTestAppointmentDto.LocalDrivingLicenseApplicationID, createTestAppointmentDto.TestTypeID);

        if (previousTest != null)
        {
            if (previousTest.Passed)
                return ServiceResult<(LocalDrivingLicenseApplication localDrivingApp, TestType testType)>.Failure([$"This applicant has already passed the {testName} for this application"], FailureType.Conflict);

            return ServiceResult<(LocalDrivingLicenseApplication localDrivingApp, TestType testType)>.Failure(["This test has already been taken. Apply for a retake instead"], FailureType.Conflict);
        }

        if (!await _testService.IsEligibleForTestAsync(createTestAppointmentDto.LocalDrivingLicenseApplicationID, testTypeId))
            return ServiceResult<(LocalDrivingLicenseApplication localDrivingApp, TestType testType)>.Failure([$"This applicant has to pass the {previousTestName} first before booking for the {testName}"], FailureType.ValidationError);

        if (await HasPendingAppointmentAsync(createTestAppointmentDto.LocalDrivingLicenseApplicationID, testTypeId))
            return ServiceResult<(LocalDrivingLicenseApplication localDrivingApp, TestType testType)>.Failure([$"This applicant already has a pending appointment for the {testName}"], FailureType.Conflict);

        return ServiceResult<(LocalDrivingLicenseApplication localDrivingApp, TestType testType)>.Success((localDrivingApp, testType));
    }

    private TestAppointment BuildTestAppointmentEntityAsync(LocalDrivingLicenseApplication localDrivingApp, TestType testType, DateTime appointmentTime)
    {
        return new TestAppointment
        {
            TestTypeID = testType.TestTypeID,
            LocalDrivingLicenseApplicationID = localDrivingApp.LocalDrivingLicenseApplicationID,
            AppointmentTime = appointmentTime,
            PaidFees = testType.TestTypeFees,
            CreatedByUserID = _currentUserService.UserID,
            IsLocked = false,
            RetakeTestApplicationID = null
        };
    }

    private async Task<bool> HasPendingAppointmentAsync(int localDrivingAppId, int testTypeId)
    {
        return await _context.TestAppointments
        .AnyAsync(testApp => testApp.LocalDrivingLicenseApplicationID == localDrivingAppId
        && testApp.TestTypeID == testTypeId
        && !testApp.IsLocked);
    }

    public async Task<ServiceResult<TestAppointmentDto>> CreateTestAppointmentAsync(CreateTestAppointmentDto createTestAppointmentDto)
    {
        ServiceResult<(LocalDrivingLicenseApplication localDrivingApp, TestType testType)> appointmentValidation = await ValidateTestAppointmentAsync(createTestAppointmentDto);

        if (!appointmentValidation.IsSuccess)
            return ServiceResult<TestAppointmentDto>.Failure(appointmentValidation.Errors, appointmentValidation.ResultType!.Value);

        LocalDrivingLicenseApplication localDrivingApp = appointmentValidation.Data.localDrivingApp;
        TestType testType = appointmentValidation.Data.testType;

        TestAppointment newTestAppointment = BuildTestAppointmentEntityAsync(localDrivingApp, testType, createTestAppointmentDto.AppointmentTime);

        await _context.TestAppointments.AddAsync(newTestAppointment);
        await _context.SaveChangesAsync();

        Application baseApplication = localDrivingApp.BaseApplication;
        Person person = baseApplication.ApplicantPerson;

        TestAppointmentDto savedTestDto = new TestAppointmentDto(
            newTestAppointment.TestAppointmentID,
            testType.TestTypeTitle,
            $"{person.FirstName} {person.LastName}",
            person.NationalNo,
            newTestAppointment.AppointmentTime,
            _currentUserService.UserName!
        );

        return ServiceResult<TestAppointmentDto>.Success(savedTestDto);
    }

    public async Task<bool> RetakeTestAlreadyBookedAsync(int localAppId, int testTypeId)
    {
        return await _context.TestAppointments.AnyAsync(ta => ta.LocalDrivingLicenseApplicationID == localAppId
        && ta.TestTypeID == testTypeId
        && ta.RetakeTestApplicationID != null);
    }
}