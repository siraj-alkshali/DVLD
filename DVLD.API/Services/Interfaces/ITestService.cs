using DVLD.API.DTOs.Tests;
using DVLD.API.Common.Results;

namespace DVLD.API.Services.Interfaces;

public interface ITestService
{
    Task<bool> IsEligibleForTestAsync(int localDrivingAppId, int testTypeId);
    Task<bool> PassedTestAsync(int localDrivingAppId, int testTypeId);
    Task<ServiceResult<TestDto>> CreateNewTestResult(CreateTestDto dto);
}