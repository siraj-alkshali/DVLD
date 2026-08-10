using DVLD.API.DTOs.Countries;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface ICountryService
{
    Task<List<CountryDto>> GetAllCountriesAsync();
    Task<Country?> GetCountryByIdAsync(int countryId);
    Task<CountryDto?> GetCountryDtoByIdAsync(int countryId);
}