using DVLD.API.DTOs.ApplicationStatuses;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class ApplicationStatusService : IApplicationStatusService
{
    private readonly DVLDContext _context;

    public ApplicationStatusService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<ApplicationStatusDto?> GetApplicationStatusByIdAsync(int id)
    {
        // return await _context.ApplicationStatuses.Where(appStatus => appStatus.ApplicationStatusID == id)
        // .Select(appStatus => new ApplicationStatusDto(
        // appStatus.ApplicationStatusID, appStatus.StatusName
        // )).SingleOrDefaultAsync();

        ApplicationStatus? appStatus = await _context.ApplicationStatuses.FindAsync(id);

        if (appStatus == null)
            return null;

        return new ApplicationStatusDto(appStatus.ApplicationStatusID, appStatus.StatusName);
    }

    public async Task<IEnumerable<ApplicationStatusDto>> GetAllApplicationStatusesAsync()
    {
        return await _context.ApplicationStatuses.Select(appStatus => new ApplicationStatusDto(
        appStatus.ApplicationStatusID, appStatus.StatusName
        )).ToListAsync();
    }

    public async Task<bool> ApplicationStatusExistsAsync(int id)
    {
        return await _context.ApplicationStatuses.AnyAsync(appStatus => appStatus.ApplicationStatusID == id);
    }
}