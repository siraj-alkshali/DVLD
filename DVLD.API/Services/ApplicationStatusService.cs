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

    public async Task<ApplicationStatusDto?> GetApplicationStatusByIdAsync(int applicationStatusId)
    {
        ApplicationStatus? appStatus = await _context.ApplicationStatuses.FindAsync(applicationStatusId);

        if (appStatus == null)
            return null;

        return new ApplicationStatusDto(appStatus.ApplicationStatusID, appStatus.StatusName);
    }

    public async Task<List<ApplicationStatusDto>> GetAllApplicationStatusesAsync()
    {
        return await _context.ApplicationStatuses
        .AsNoTracking()
        .Select(appStatus => new ApplicationStatusDto(appStatus.ApplicationStatusID, appStatus.StatusName))
        .ToListAsync();
    }
}