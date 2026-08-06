using DVLD.API.DTOs.Countries;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/genders")]
public class GendersController : ControllerBase
{

    private readonly IGenderService _genderService;

    public GendersController(IGenderService genderService)
    {
        _genderService = genderService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GenderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GenderDto>>> GetAllGenders()
    {
        return Ok(await _genderService.GetAllGendersAsync());
    }

    [HttpGet("{id}", Name = "GetGenderById")]
    [ProducesResponseType(typeof(GenderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenderDto>> GetGenderById(int id)
    {
        GenderDto? gender = await _genderService.GetGenderDtoByIdAsync(id);

        if (gender == null)
            return NotFound();

        return Ok(gender);
    }
}