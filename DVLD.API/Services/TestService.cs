using DVLD.API.Common.Constants;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class TestService : ITestService
{

    private readonly DVLDContext _context;

    public TestService(DVLDContext context)
    {
        _context = context;
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
}