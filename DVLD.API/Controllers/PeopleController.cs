using DVLD.API.DTOs.People;
using DVLD.API.Common.Results;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DVLD.API.DTOs.Common;
using DVLD.API.Common.QueryParameters;
using DVLD.API.Extensions;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/people")]
public class PeopleController : ControllerBase
{
    private readonly IPersonService _personService;

    public PeopleController(IPersonService personService)
    {
        _personService = personService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<PersonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<PersonDto>>> GetAllPeople([FromQuery] PeopleQueryParameters parameters)
    {
        return Ok(await _personService.GetAllPeopleAsync(parameters));
    }

    [HttpGet("{personId}", Name = "GetPersonById")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> GetPersonById(int personId)
    {
        PersonDto? person = await _personService.GetPersonDtoByIdAsync(personId);

        if (person == null)
            return NotFound();

        return Ok(person);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PersonDto>> CreatePerson(CreatePersonDto createPersonDto)
    {
        ServiceResult<PersonDto> result = await _personService.CreatePersonAsync(createPersonDto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return CreatedAtRoute("GetPersonById", new { personId = result.Data!.PersonID }, result.Data
        );
    }

    [HttpPut("{personId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdatePerson(int personId, UpdatePersonDto updatePersonDto)
    {
        ServiceResult<PersonDto> result = await _personService.UpdatePersonAsync(personId, updatePersonDto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return Ok(result.Data);
    }

    [HttpPost("{personId}/image")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> UpdatePersonImage(int personId, IFormFile image)
    {
        ServiceResult<PersonDto> result = await _personService.UpdatePersonImageAsync(personId, image);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return Ok(result.Data);
    }

    [HttpDelete("{personId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePerson(int personId)
    {
        bool deleted = await _personService.DeletePersonAsync(personId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("nationalNo/{nationalNo}", Name = "GetPersonByNationalNo")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> GetPersonByNationalNo(string nationalNo)
    {
        PersonDto? person = await _personService.GetPersonDtoByNationalNoAsync(nationalNo);

        if (person == null)
            return NotFound();

        return Ok(person);
    }

}