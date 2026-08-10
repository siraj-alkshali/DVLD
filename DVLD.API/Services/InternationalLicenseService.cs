using DVLD.API.Common.QueryParameters;
using DVLD.API.DTOs;
using DVLD.API.DTOs.Common;
using DVLD.API.Extensions;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class InternationalLicenseService : IInternationalLicenseService
{
    private readonly DVLDContext _context;

    public InternationalLicenseService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<InternationalLicenseDto>> GetAllInternationalLicensesAsync(InternationalLicensesQueryParameters parameters)
    {
        IQueryable<InternationalLicense> query = _context.InternationalLicenses
        .AsNoTracking()
        .Include(intlLicense => intlLicense.Driver)
        .ThenInclude(intlLicense => intlLicense.Person)
        .Include(intlLicense => intlLicense.IssuedUsingLocalLicense)
        .ThenInclude(intlLicense => intlLicense.LicenseClass)
        .ApplySearch(parameters.SearchTerm)
        .ApplyFilter(parameters)
        .ApplySort(parameters);

        int totalItems = await query.CountAsync();

        List<InternationalLicenseDto> items = await query.ApplyPagination(parameters)
        .Select(intlLicense => new InternationalLicenseDto(
            intlLicense.InternationalLicenseID,
            $"{intlLicense.Driver.Person.FirstName} {intlLicense.Driver.Person.LastName}",
            intlLicense.Driver.Person.NationalNo,
            intlLicense.Driver.Person.Phone,
            intlLicense.IssuedUsingLocalLicense.LicenseClass.ClassName,
            intlLicense.IssueDate,
            intlLicense.ExpirationDate,
            intlLicense.IsActive
        )).ToListAsync();

        return new PagedResultDto<InternationalLicenseDto>
        {
            Items = items,
            TotalItems = totalItems,
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber
        };
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