using DVLD.API.DTOs.Countries;

namespace DVLD.API.Services.Interfaces;

public interface ICountryService
{
    Task<IEnumerable<CountryDto>> GetAllCountriesAsync();
    Task<CountryDto?> GetCountryByIdAsync(int id);
    Task<bool> CountryExistsAsync(int id);
}