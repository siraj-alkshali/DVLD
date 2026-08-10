using DVLD.API.DTOs.Countries;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class CountryService : ICountryService
{

    private readonly DVLDContext _context;

    public CountryService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<List<CountryDto>> GetAllCountriesAsync()
    {
        return await _context.Countries
        .AsNoTracking()
        .Select(c => new CountryDto(c.CountryID, c.CountryName))
        .ToListAsync();
    }

    public async Task<Country?> GetCountryByIdAsync(int countryId)
    {
        return await _context.Countries.FindAsync(countryId);
    }

    public async Task<CountryDto?> GetCountryDtoByIdAsync(int countryId)
    {
        return await _context.Countries
        .AsNoTracking()
        .Where(c => c.CountryID == countryId)
        .Select(c => new CountryDto(c.CountryID, c.CountryName))
        .SingleOrDefaultAsync();
    }
}