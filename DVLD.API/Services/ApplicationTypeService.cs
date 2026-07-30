using DVLD.API.DTOs.ApplicationTypes;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class ApplicationTypeService : IApplicationTypeService
{
    private readonly DVLDContext _context;

    public ApplicationTypeService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicationTypeDto>> GetAllApplicationTypesAsync()
    {
        return await _context.ApplicationTypes.Select(appType => new ApplicationTypeDto(
            appType.ApplicationTypeID, appType.ApplicationTypeTitle, appType.ApplicationFees
        )).ToListAsync();
    }

    public async Task<ApplicationTypeDto?> GetApplicationTypeByIdAsync(int id)
    {
        // return await _context.ApplicationTypes.Where(appType => appType.ApplicationTypeID == id)
        // .Select(appType => new ApplicationTypeDto(
        // appType.ApplicationTypeID, appType.ApplicationTypeTitle, appType.ApplicationFees
        // )).SingleOrDefaultAsync();

        ApplicationType? applicationType = await _context.ApplicationTypes.FindAsync(id);

        if (applicationType == null)
            return null;

        return new ApplicationTypeDto(applicationType.ApplicationTypeID, applicationType.ApplicationTypeTitle, applicationType.ApplicationFees);
    }

    public async Task<bool> ApplicationTypeExists(int id)
    {
        return await _context.ApplicationTypes.AnyAsync(appType => appType.ApplicationTypeID == id);
    }
}