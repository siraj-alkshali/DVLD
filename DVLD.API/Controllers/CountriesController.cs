using DVLD.API.DTOs.Countries;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/countries")]
public class CountriesController : ControllerBase
{

    private readonly ICountryService _countryService;

    public CountriesController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CountryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CountryDto>>> GetAllCountries()
    {
        return Ok(await _countryService.GetAllCountriesAsync());
    }

    [HttpGet("{countryId}", Name = "GetCountryById")]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CountryDto>> GetCountryById(int countryId)
    {
        CountryDto? country = await _countryService.GetCountryDtoByIdAsync(countryId);

        if (country == null)
            return NotFound();

        return Ok(country);
    }
}