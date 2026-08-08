using DVLD.API.DTOs;
using DVLD.API.Mappings.People;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class InternationalLicenseService : IInternationalLicenseService
{
    private readonly DVLDContext _context;

    public InternationalLicenseService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<InternationalLicenseDto?> GetInternationalLicenseDtoByIdAsync(int internationalLicenseId)
    {
        return await _context.InternationalLicenses
        .AsNoTracking()
        .Where(intl => intl.InternationalLicenseID == internationalLicenseId)
        .Select(intl => new InternationalLicenseDto(
            intl.InternationalLicenseID,
            $"{intl.Driver.Person.FirstName} {intl.Driver.Person.LastName}",
            intl.Driver.Person.NationalNo,
            intl.Driver.Person.Phone,
            intl.IssuedUsingLocalLicense.LicenseClass.ClassName,
            intl.IssueDate,
            intl.ExpirationDate,
            intl.IsActive
        ))
        .SingleOrDefaultAsync();
    }

    public async Task<bool> ActiveInternationalLicenseExistsAsync(int driverId)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        return await _context.InternationalLicenses.AnyAsync(il => il.DriverID == driverId
        && il.IsActive
        && il.ExpirationDate < today);
    }
}