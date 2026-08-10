using DVLD.API.DTOs.LicenseIssueReasons;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class LicenseIssueReasonService : ILicenseIssueReasonService
{
    private readonly DVLDContext _context;

    public LicenseIssueReasonService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<List<LicenseIssueReasonDto>> GetAllLicenseIssueReasonsAsync()
    {
        return await _context.LicenseIssueReasons
        .AsNoTracking()
        .Select(lir => new LicenseIssueReasonDto(lir.IssueReasonID, lir.IssueReasonName))
        .ToListAsync();
    }

    public async Task<LicenseIssueReasonDto?> GetLicenseIssueReasonById(int licenseIssueReasonId)
    {
        return await _context.LicenseIssueReasons
        .Where(lir => lir.IssueReasonID == licenseIssueReasonId)
        .Select(lir => new LicenseIssueReasonDto(lir.IssueReasonID, lir.IssueReasonName))
        .SingleOrDefaultAsync();
    }
}