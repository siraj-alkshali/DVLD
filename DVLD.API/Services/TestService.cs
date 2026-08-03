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

    public async Task<ServiceResult<TestDto>> CreateNewTestResult(CreateTestDto dto)
    {
        TestAppointment? testApp = await _context.TestAppointments
        .Include(testApp => testApp.LocalDrivingLicenseApplication)
        .ThenInclude(localApp => localApp.BaseApplication)
        .SingleOrDefaultAsync(testApp => testApp.TestAppointmentID == dto.TestAppointmentID);

        if (testApp == null)
            return ServiceResult<TestDto>.Failure(["This test appointment doesn't exist"], FailureType.Conflict);

        string testName = ((enTestType)testApp.TestTypeID).GetDisplayName();
        string previousTestName = "";

        if (testApp.TestTypeID != (int)enTestType.VisionTest)
            previousTestName = ((enTestType)testApp.TestTypeID - 1).GetDisplayName();

        if (testApp.IsLocked)
            return ServiceResult<TestDto>.Failure(["A result has already been recorded for this appointment"], FailureType.Conflict);

        // if (testApp.AppointmentTime > DateTime.Now)
        //     return ServiceResult<TestDto>.Failure(["Cannot submit a test result before the appointment time"], FailureType.Conflict);

        if (!_localDrivingLicenseApplicationService.IsActiveAsync(testApp.LocalDrivingLicenseApplication))
            return ServiceResult<TestDto>.Failure(["Cannot schedule a test appointment because the application is not active"], FailureType.Conflict);

        if (await TestExistsAsync(testApp.TestAppointmentID))
            return ServiceResult<TestDto>.Failure([$"This test has already been taken"], FailureType.Conflict);

        int? createdByUserID = _currentUserService.UserID;

        if (createdByUserID == null)
            return ServiceResult<TestDto>.Failure(["Unable to determine current user"], FailureType.Unauthorized);

        Test test = new Test
        {
            TestAppointmentID = testApp.TestAppointmentID,
            Passed = dto.Passed,
            Notes = dto.Notes,
            CreatedByUserID = createdByUserID.Value
        };

        await _context.Tests.AddAsync(test);
        testApp.IsLocked = true;
        await _context.SaveChangesAsync();

        Test savedTest = await _context.Tests
        .AsNoTracking()
        .Where(t => t.TestID == test.TestID)
        .Include(t => t.TestAppointment)
        .ThenInclude(ta => ta.LocalDrivingLicenseApplication)
        .ThenInclude(localApp => localApp.BaseApplication)
        .ThenInclude(baseApp => baseApp.ApplicantPerson)
        .Include(t => t.TestAppointment)
        .ThenInclude(ta => ta.TestType)
        .Include(t => t.CreatedByUser)
        .SingleAsync();

        return ServiceResult<TestDto>.Success(savedTest.ToDto());

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
}