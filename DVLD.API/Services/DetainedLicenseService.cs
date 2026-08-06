using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class DetainedLicenseService : IDetainedLicenseService
{
    private readonly DVLDContext _context;

    public DetainedLicenseService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<bool> IsLicenseDetainedAsync(int licenseId)
    {
        return await _context.DetainedLicenses.AnyAsync(dl => dl.LicenseID == licenseId
        && dl.ReleaseDate == null);
    }
}