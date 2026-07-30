using DVLD.API.DTOs.LicenseClasses;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class LicenseClassService : ILicenseClassService
{

    private readonly DVLDContext _context;

    public LicenseClassService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LicenseClassDto>> GetAllLicenseClassesAsync()
    {
        return await _context.LicenseClasses.Select(lc => new LicenseClassDto(lc.LicenseClassID, lc.ClassName, lc.ClassDescription, lc.MinimumAllowedAge, lc.DefaultValidityLength, lc.ClassFees))
        .ToListAsync();
    }

    public async Task<LicenseClassDto?> GetLicenseClassByIdAsync(int id)
    {
        return await _context.LicenseClasses.Where(lc => lc.LicenseClassID == id)
        .Select(lc => new LicenseClassDto(lc.LicenseClassID, lc.ClassName, lc.ClassDescription, lc.MinimumAllowedAge, lc.DefaultValidityLength, lc.ClassFees))
        .SingleOrDefaultAsync();
    }

    public async Task<bool> LicenseClassExistsAsync(int id)
    {
        return await _context.LicenseClasses.AnyAsync(lc => lc.LicenseClassID == id);
    }
}