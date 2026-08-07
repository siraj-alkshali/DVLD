using DVLD.API.Common.Constants;
using DVLD.API.Common.Results;
using DVLD.API.Extensions;
using DVLD.API.DTOs.Tests;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using DVLD.API.Mappings.Tests;

namespace DVLD.API.Services;

public class TestService : ITestService
{

    private readonly DVLDContext _context;
    private readonly ILocalDrivingLicenseApplicationService _localDrivingLicenseApplicationService;
    private readonly ICurrentUserService _currentUserService;

    public TestService(DVLDContext context, ILocalDrivingLicenseApplicationService localDrivingLicenseApplicationService, ICurrentUserService currentUserService)
    {
        _context = context;
        _localDrivingLicenseApplicationService = localDrivingLicenseApplicationService;
        _currentUserService = currentUserService;
    }

    private async Task<ServiceResult<TestAppointment>> ValidateAndGetTestAppointment(CreateTestDto createTestDto)
    {
        TestAppointment? testApp = await _context.TestAppointments
        .Include(testApp => testApp.LocalDrivingLicenseApplication)
        .ThenInclude(localApp => localApp.BaseApplication)
        .ThenInclude(baseApp => baseApp.ApplicantPerson)
        .Include(t => t.TestType)
        .SingleOrDefaultAsync(testApp => testApp.TestAppointmentID == createTestDto.TestAppointmentID);

        if (testApp == null)
            return ServiceResult<TestAppointment>.Failure(["This test appointment doesn't exist"], FailureType.NotFound);

        if (testApp.IsLocked)
            return ServiceResult<TestAppointment>.Failure(["A result has already been recorded for this appointment"], FailureType.Conflict);

        // if (testApp.AppointmentTime > DateTime.Now)
        //     return ServiceResult<TestDto>.Failure(["Cannot submit a test result before the appointment time"], FailureType.Conflict);

        if (!_localDrivingLicenseApplicationService.IsActive(testApp.LocalDrivingLicenseApplication))
            return ServiceResult<TestAppointment>.Failure(["Cannot schedule a test appointment because the application is not active"], FailureType.Conflict);

        if (await TestExistsAsync(testApp.TestAppointmentID))
            return ServiceResult<TestAppointment>.Failure([$"This test has already been taken"], FailureType.Conflict);

        return ServiceResult<TestAppointment>.Success(testApp);
    }

    private Test BuildTestEntity(TestAppointment testApp, CreateTestDto createTestDto)
    {
        return new Test
        {
            TestAppointmentID = testApp.TestAppointmentID,
            Passed = createTestDto.Passed,
            Notes = createTestDto.Notes,
            CreatedByUserID = _currentUserService.UserID
        };
    }
    public async Task<ServiceResult<TestDto>> CreateNewTestResult(CreateTestDto createTestDto)
    {

        ServiceResult<TestAppointment> testAppointmentValidation = await ValidateAndGetTestAppointment(createTestDto);

        if (!testAppointmentValidation.IsSuccess)
            return ServiceResult<TestDto>.Failure(testAppointmentValidation.Errors, testAppointmentValidation.ResultType!.Value);

        TestAppointment testApp = testAppointmentValidation.Data!;

        Test test = BuildTestEntity(testApp, createTestDto);
        LocalDrivingLicenseApplication localDrivingApp = testApp.LocalDrivingLicenseApplication;
        Application baseApplication = localDrivingApp.BaseApplication;
        Person person = baseApplication.ApplicantPerson;
        TestType testType = testApp.TestType;

        await _context.Tests.AddAsync(test);
        testApp.IsLocked = true;

        if (testApp.RetakeTestApplicationID != null)
        {
            baseApplication.ApplicationStatusID = (int)enApplicationStatus.Completed;
            baseApplication.LastStatusDate = DateOnly.FromDateTime(DateTime.Now);
        }

        await _context.SaveChangesAsync();

        TestDto savedTestDto = new TestDto(
            test.TestID,
            testType.TestTypeTitle,
            $"{person.FirstName} {person.LastName}",
            person.NationalNo,
            testApp.AppointmentTime,
            test.Passed,
            test.Notes,
            _currentUserService.UserName
        );

        return ServiceResult<TestDto>.Success(savedTestDto);
    }

    public async Task<bool> IsEligibleForTestAsync(int localDrivingAppId, int testTypeId)
    {
        if (testTypeId == (int)enTestType.VisionTest)
            return true;

        int previousTestTypeId = testTypeId - 1;

        return await _context.Tests
        .AnyAsync(t => t.TestAppointment.LocalDrivingLicenseApplicationID == localDrivingAppId
        && t.TestAppointment.TestTypeID == previousTestTypeId
        && t.Passed);
    }

    public async Task<bool> PassedTestAsync(int localDrivingAppId, int testTypeId)
    {
        return await _context.Tests
        .AnyAsync(t => t.TestAppointment.LocalDrivingLicenseApplicationID == localDrivingAppId
        && t.TestAppointment.TestTypeID == testTypeId
        && t.Passed);
    }

    private async Task<bool> TestExistsAsync(int appointmentId)
    {
        return await _context.Tests.AnyAsync(t => t.TestAppointmentID == appointmentId);
    }

    public async Task<Test?> GetTestWithDetailsByTestIdAsync(int testId)
    {
        return await _context.Tests.Include(t => t.TestAppointment)
        .ThenInclude(ta => ta.LocalDrivingLicenseApplication)
        .ThenInclude(la => la.BaseApplication)
        .ThenInclude(ba => ba.ApplicantPerson)
        .Include(t => t.TestAppointment)
        .ThenInclude(t => t.TestType)
        .SingleOrDefaultAsync(t => t.TestID == testId);
    }

    public async Task<bool> PassedAllRequiredTestsAsync(int localDrivingAppId)
    {
        return await _context.Tests
            .Where(t => t.TestAppointment.LocalDrivingLicenseApplicationID == localDrivingAppId
            && t.Passed)
            .Select(t => t.TestAppointment.TestTypeID)
            .Distinct()
            .CountAsync() == 3;
    }
}