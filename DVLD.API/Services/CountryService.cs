using DVLD.API.DTOs.Countries;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class CountryService : ICountryService
{

    private readonly DVLDContext _context;

    public CountryService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync()
    {
        return await _context.Countries.Select(c => new CountryDto(c.CountryID, c.CountryName))
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<CountryDto?> GetCountryByIdAsync(int id)
    {
        return await _context.Countries
        .AsNoTracking()
        .Where(c => c.CountryID == id)
        .Select(c => new CountryDto(c.CountryID, c.CountryName))
        .SingleOrDefaultAsync();
    }

    public async Task<bool> CountryExistsAsync(int id)
    {
        return await _context.Countries.AnyAsync(c => c.CountryID == id);
    }
}