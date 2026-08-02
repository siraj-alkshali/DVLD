using DVLD.API.DTOs.People;
using DVLD.API.Common.Results;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DVLD.API.DTOs.Common;
using DVLD.API.Common.QueryParameters;

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
        PagedResultDto<PersonDto> pagedResults = await _personService.GetAllPeopleAsync(parameters);

        return Ok(pagedResults);
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePerson(int id, UpdatePersonDto updatePersonDto)
    {
        ServiceResult<PersonDto> result = await _personService.UpdatePersonAsync(id, updatePersonDto);

        if (!result.IsSuccess)
        {
            switch (result.ResultType)
            {
                case FailureType.Conflict:
                    return Conflict(result.Errors);

                case FailureType.NotFound:
                    return NotFound();

                default:
                    return StatusCode(500);
            }
        }

        return Ok(result.Data);
    }

    [HttpPost("{id}/image")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> UpdatePersonImage(int id, IFormFile image)
    {
        ServiceResult<PersonDto> result = await _personService.UpdatePersonImageAsync(id, image);

        if (!result.IsSuccess)
        {
            switch (result.ResultType)
            {
                case FailureType.Validation:
                    return BadRequest(result.Errors);

                case FailureType.NotFound:
                    return NotFound();

                default:
                    return StatusCode(500);
            }
        }

        return Ok(result.Data);
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
    public async Task<ActionResult<PersonDto>> GetPersonByNationalNo(string nationalNo)
    {
        PersonDto? person = await _personService.GetPersonByNationalNoAsync(nationalNo);

        if (person == null)
        {
            return NotFound();
        }

        return Ok(person);
    }

}