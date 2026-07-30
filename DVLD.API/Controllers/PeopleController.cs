using DVLD.API.DTOs.People;
using DVLD.API.Common.Results;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetAllPeople()
    {
        IEnumerable<PersonDto> people = await _personService.GetAllPeopleAsync();

        return Ok(people);
    }

    [HttpGet("{id}", Name = "GetPersonById")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> GetPersonById(int id)
    {
        PersonDto? person = await _personService.GetPersonByIdAsync(id);

        if (person == null)
        {
            return NotFound();
        }

        return Ok(person);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PersonDto>> CreatePerson(CreatePersonDto createPersonDto)
    {
        ServiceResult<PersonDto> result = await _personService.CreatePersonAsync(createPersonDto);

        if (!result.IsSuccess)
            return Conflict(result.Errors);

        return CreatedAtRoute(
            "GetPersonById",
            new { id = result.Data!.PersonID },
            result.Data
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePerson(int id, UpdatePersonDto updatePersonDto)
    {
        bool updated = await _personService.UpdatePersonAsync(id, updatePersonDto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePerson(int id)
    {
        bool deleted = await _personService.DeletePersonAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("nationalNo/{nationalNo}", Name = "GetPersonByNationalNo")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPersonByNationalNo(string nationalNo)
    {
        PersonDto? person = await _personService.GetPersonByNationalNoAsync(nationalNo);

        if (person == null)
        {
            return NotFound();
        }

        return Ok(person);
    }

}