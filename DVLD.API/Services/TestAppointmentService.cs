using DVLD.API.Common.Constants;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.TestAppointments;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.API.Extensions;
using DVLD.API.Mappings.TestAppointments;
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

    private async Task<ServiceResult<bool>> ValidateTestAppointmentAsync(int localDrivingAppId, int testTypeId)
    {
        string testName = ((enTestType)testTypeId).GetDisplayName();
        string previousTestName = "";

        if (testTypeId != (int)enTestType.VisionTest)
            previousTestName = ((enTestType)testTypeId - 1).GetDisplayName();

        LocalDrivingLicenseApplication? localDrivingApp = await _localDrivingLicenseApplicationService.GetLocalDrivingLicenseApplicationById(localDrivingAppId);

        if (localDrivingApp == null)
            return ServiceResult<bool>.Failure(["This local driving license application doesn't exist"], FailureType.Conflict);

        if (!_localDrivingLicenseApplicationService.IsActiveAsync(localDrivingApp))
            return ServiceResult<bool>.Failure(["Cannot schedule a test appointment because the application is not active"], FailureType.Conflict);

        if (await _testService.PassedTestAsync(localDrivingAppId, testTypeId))
            return ServiceResult<bool>.Failure([$"This applicant has already passed the {testName} for this application"], FailureType.Conflict);

        if (!await _testService.IsEligibleForTestAsync(localDrivingAppId, testTypeId))
            return ServiceResult<bool>.Failure([$"This applicant has to pass the {previousTestName} first before booking for the {testName}"], FailureType.Conflict);

        if (await HasPendingAppointmentAsync(localDrivingAppId, testTypeId))
            return ServiceResult<bool>.Failure([$"This applicant already has a pending appointment for the {testName}"], FailureType.Conflict);

        return ServiceResult<bool>.Success(true);
    }

    private async Task<ServiceResult<TestAppointment>> BuildTestAppointmentEntityAsync(int localDrivingAppId, int testTypeId, DateTime appointmentTime)
    {
        TestType? testType = await _testTypeService.GetTestTypeByIdAsync(testTypeId);

        if (testType == null)
            return ServiceResult<TestAppointment>.Failure(["This test type does not exist"], FailureType.Conflict);

        ServiceResult<bool> validationResult =
            await ValidateTestAppointmentAsync(localDrivingAppId, testTypeId);

        if (!validationResult.IsSuccess)
        {
            return ServiceResult<TestAppointment>.Failure(
                validationResult.Errors,
                validationResult.ResultType!.Value);
        }

        int? createdByUserID = _currentUserService.UserID;

        if (createdByUserID == null)
            return ServiceResult<TestAppointment>.Failure(["Unable to determine current user"], FailureType.Unauthorized);

        TestAppointment testAppointment = new TestAppointment
        {
            TestTypeID = testTypeId,
            LocalDrivingLicenseApplicationID = localDrivingAppId,
            AppointmentTime = appointmentTime,
            PaidFees = testType.TestTypeFees,
            CreatedByUserID = createdByUserID.Value,
            IsLocked = false,
            RetakeTestApplicationID = null
        };

        return ServiceResult<TestAppointment>.Success(testAppointment);
    }

    public async Task<ServiceResult<TestAppointmentDto>> CreateTestAppointmentAsync(CreateTestAppointmentDto dto)
    {
        ServiceResult<TestAppointment> testAppointmentEntityCreationResult = await BuildTestAppointmentEntityAsync(dto.LocalDrivingLicenseApplicationID, dto.TestTypeID, dto.AppointmentTime);

        if (!testAppointmentEntityCreationResult.IsSuccess)
            return ServiceResult<TestAppointmentDto>.Failure(testAppointmentEntityCreationResult.Errors, testAppointmentEntityCreationResult.ResultType!.Value);

        TestAppointment testAppointment = testAppointmentEntityCreationResult.Data!;
        await _context.TestAppointments.AddAsync(testAppointment);
        await _context.SaveChangesAsync();

        TestAppointment savedTestAppointment = await _context.TestAppointments
        .AsNoTracking()
        .Where(t => t.TestAppointmentID == testAppointment.TestAppointmentID)
        .Include(t => t.LocalDrivingLicenseApplication)
        .ThenInclude(localApp => localApp.BaseApplication)
        .ThenInclude(baseApp => baseApp.ApplicantPerson)
        .Include(t => t.CreatedByUser)
        .Include(t => t.TestType)
        .SingleAsync();

        return ServiceResult<TestAppointmentDto>.Success(savedTestAppointment.ToDto());
    }

    private async Task<bool> HasPendingAppointmentAsync(int localDrivingAppId, int testTypeId)
    {
        return await _context.TestAppointments
        .AnyAsync(testApp => testApp.LocalDrivingLicenseApplicationID == localDrivingAppId
        && testApp.TestTypeID == testTypeId
        && !testApp.IsLocked);
    }
}