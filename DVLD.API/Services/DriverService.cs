using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class DriverService : IDriverService
{
    private readonly DVLDContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DriverService(DVLDContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private async Task<Driver?> GetDriverByPersonIdAsync(int personId)
    {
        return await _context.Drivers.SingleOrDefaultAsync(d => d.PersonID == personId);
    }

    public async Task<(Driver Driver, bool IsNew)> GetOrCreateDriverAsync(int personId)
    {
        Driver? driver = await GetDriverByPersonIdAsync(personId);

        if (driver != null)
            return (driver, false);

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        int? createdByUserID = _currentUserService.UserID;

        if (createdByUserID is null)
            throw new InvalidOperationException("Current user ID is not available.");

        return (new Driver
        {
            PersonID = personId,
            CreatedByUserID = createdByUserID.Value,
            CreatedDate = today
        }, true);
    }
}