using DVLD.API.DTOs.People;
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

    [HttpPost]
    public async Task<IActionResult> CreatePerson(CreatePersonDto dto)
    {
        int personID = await _personService.CreatePersonAsync(dto);

        // return CreatedAtAction(
        //     nameof(GetPersonById),
        //     new { id = personID },
        //     personID
        // );

        return Ok(personID);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPersonById(int id)
    {
        PersonDto? person = await _personService.GetPersonByIDAsync(id);

        if (person == null)
        {
            return NotFound();
        }

        return Ok(person);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPeople()
    {
        IEnumerable<PersonDto> people = await _personService.GetAllPeopleAsync();

        return Ok(people);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePerson(int id, UpdatePersonDto dto)
    {
        var updated = await _personService.UpdatePersonAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson(int id)
    {
        var deleted = await _personService.DeletePersonAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("search/nationalNo/{nationalNo}")]
    public async Task<IActionResult> SearchByNationalNo(string nationalNo)
    {
        var person = await _personService.GetPersonByNationalNoAsync(nationalNo);

        if (person == null)
        {
            return NotFound();
        }

        return Ok(person);
    }

}