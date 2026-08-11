using DVLD.API.Common.QueryParameters;
using DVLD.API.DTOs;
using DVLD.API.DTOs.Common;
using DVLD.API.Extensions;
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

    public async Task<PagedResultDto<DriverDto>> GetAllDriversAsync(DriversQueryParameters parameters)
    {
        IQueryable<Driver> query = _context.Drivers
        .AsNoTracking()
        .ApplySearch(parameters.SearchTerm)
        .ApplySort(parameters);

        int totalItems = await query.CountAsync();

        List<DriverDto> items = await query.ApplyPagination(parameters)
        .Select(driver => new DriverDto(
            driver.DriverID,
            $"{driver.Person.FirstName} {driver.Person.LastName}",
            driver.Person.NationalNo,
            driver.Person.Phone,
            driver.CreatedByUser.UserName
        )).ToListAsync();

        return new PagedResultDto<DriverDto>
        {
            Items = items,
            TotalItems = totalItems,
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber
        };
    }

    public async Task<(Driver Driver, bool IsNew)> GetOrCreateDriverAsync(int personId)
    {
        Driver? driver = await GetDriverByPersonIdAsync(personId);

        if (driver != null)
            return (driver, false);

        return (new Driver
        {
            PersonID = personId,
            CreatedByUserID = _currentUserService.UserID,
            CreatedDate = DateOnly.FromDateTime(DateTime.Now)
        }, true);
    }
}