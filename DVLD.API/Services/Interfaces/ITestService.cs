namespace DVLD.API.Services.Interfaces;

public interface ITestService
{
    Task<bool> IsEligibleForTestAsync(int localDrivingAppId, int testTypeId);
    Task<bool> PassedTestAsync(int localDrivingAppId, int testTypeId);
}