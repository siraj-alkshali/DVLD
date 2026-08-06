using DVLD.API.Common.Constants;
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

    public async Task<decimal> GetApplicationTypeFeesAsync(enApplicationType applicationType)
    {
        return await _context.ApplicationTypes
        .Where(appType => appType.ApplicationTypeID == (int)applicationType)
        .Select(appType => appType.ApplicationFees)
        .SingleAsync();
    }
}