using DVLD.API.DTOs.Tests;
using DVLD.API.Common.Results;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface ITestService
{
    Task<bool> IsEligibleForTestAsync(int localDrivingAppId, int testTypeId);
    Task<bool> PassedTestAsync(int localDrivingAppId, int testTypeId);
    Task<bool> HasAlreadyTakenTestAsync(int localDrivingAppId, int testTypeId);
    Task<ServiceResult<TestDto>> CreateNewTestResultAsync(CreateTestDto dto);
    Task<TestDto?> GetTestDtoByIdAsync(int testId);
    Task<List<TestListItemDto>> GetAllTestsForLocalDrivingAppAsync(int localDrivingAppId);
    Task<Test?> GetTestWithDetailsByTestIdAsync(int testId);
    Task<bool> PassedAllRequiredTestsAsync(int localDrivingAppId);
    Task<Test?> GetTestForTestAppointmentAsync(int localDrivingAppId, int testTypeId);
}