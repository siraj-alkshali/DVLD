using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.API.DTOs.People;
using DVLD.DataAccess.Entities;
using DVLD.API.Common.Results;
using Microsoft.EntityFrameworkCore;
using DVLD.API.Mappings.People;

namespace DVLD.API.Services;

public class PersonService : IPersonService
{
    private readonly DVLDContext _context;
    private readonly ICountryService _countryService;
    private readonly IGenderService _genderService;

    public PersonService(DVLDContext context, ICountryService countryService, IGenderService genderService)
    {
        _context = context;
        _countryService = countryService;
        _genderService = genderService;
    }

    private async Task<bool> NationalNoExistsAsync(string nationalNo)
    {
        return await _context.People.AnyAsync(p => p.NationalNo == nationalNo);
    }

    private async Task<bool> CheckDuplicateNationalNoAsync(string nationalNo, int id)
    {
        return await _context.People.AnyAsync(p => p.NationalNo == nationalNo && p.PersonID != id);
    }

    private async Task<List<string>> GetCreatePersonValidationErrorsAsync(CreatePersonDto createPersonDto)
    {
        List<string> errors = new List<string>();

        if (await NationalNoExistsAsync(createPersonDto.NationalNo))
            errors.Add("A person with this national number already exists");

        if (!await _genderService.GenderExistsAsync(createPersonDto.GenderID))
            errors.Add("The selected gender does not exist");

        if (!await _countryService.CountryExistsAsync(createPersonDto.NationalityCountryID))
            errors.Add("The selected nationality country does not exist");

        return errors;
    }

    private async Task<List<string>> GetUpdatePersonValidationErrorsAsync(UpdatePersonDto updatePersonDto, int id)
    {
        List<string> errors = new List<string>();

        if (await CheckDuplicateNationalNoAsync(updatePersonDto.NationalNo, id))
            errors.Add("A person with this national number already exists");

        if (!await _genderService.GenderExistsAsync(updatePersonDto.GenderID))
            errors.Add("The selected gender does not exist");

        if (!await _countryService.CountryExistsAsync(updatePersonDto.NationalityCountryID))
            errors.Add("The selected nationality country does not exist");

        return errors;
    }

    public async Task<IEnumerable<PersonDto>> GetAllPeopleAsync()
    {
        return await _context.People
        .AsNoTracking()
        .Select(p => new PersonDto(
        p.PersonID,
        p.NationalNo,
        $"{p.FirstName} {p.SecondName} {p.ThirdName} {p.LastName}",
        p.DateOfBirth,
        p.Gender.GenderName,
        p.Address,
        p.Phone,
        p.Email,
        p.NationalityCountry.CountryName
    )).ToListAsync();
    }

    public async Task<PersonDto?> GetPersonByIdAsync(int id)
    {
        Person? person = await _context.People.Where(p => p.PersonID == id)
                        .AsNoTracking()
                        .Include(p => p.Gender)
                        .Include(p => p.NationalityCountry)
                        .SingleOrDefaultAsync();

        if (person == null)
            return null;

        return person.ToDto();
    }

    public async Task<ServiceResult<PersonDto>> CreatePersonAsync(CreatePersonDto createPersonDto)
    {
        List<string> errors = await GetCreatePersonValidationErrorsAsync(createPersonDto);

        if (errors.Count > 0)
            return ServiceResult<PersonDto>.Failure(errors, FailureType.Conflict);

        Person person = createPersonDto.ToEntity();

        await _context.People.AddAsync(person);

        await _context.SaveChangesAsync();

        Person savedPerson = await _context.People.Where(p => p.PersonID == person.PersonID)
                        .AsNoTracking()
                        .Include(p => p.Gender)
                        .Include(p => p.NationalityCountry)
                        .SingleAsync();

        return ServiceResult<PersonDto>.Success(savedPerson.ToDto());
    }

    public async Task<ServiceResult<PersonDto>> UpdatePersonAsync(int id, UpdatePersonDto updatePersonDto)
    {

        Person? person = await _context.People
            .FindAsync(id);

        if (person == null)
        {
            return ServiceResult<PersonDto>.Failure(["The requested person was not found"], FailureType.NotFound);
        }

        List<string> errors = await GetUpdatePersonValidationErrorsAsync(updatePersonDto, id);

        if (errors.Count > 0)
            return ServiceResult<PersonDto>.Failure(errors, FailureType.Conflict);

        person.UpdateFromDto(updatePersonDto);

        await _context.SaveChangesAsync();

        Person savedPerson = await _context.People.AsNoTracking()
        .Where(p => p.PersonID == person.PersonID)
        .Include(p => p.Gender)
        .Include(p => p.NationalityCountry)
        .SingleAsync();

        return ServiceResult<PersonDto>.Success(savedPerson.ToDto());
    }

    public async Task<bool> DeletePersonAsync(int id)
    {
        Person? person = await _context.People
            .FindAsync(id);

        if (person == null)
        {
            return false;
        }

        _context.People.Remove(person);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<PersonDto?> GetPersonByNationalNoAsync(string nationalNo)
    {
        Person? person = await _context.People.Where(p => p.NationalNo == nationalNo)
                        .AsNoTracking()
                        .Include(p => p.Gender)
                        .Include(p => p.NationalityCountry)
                        .SingleOrDefaultAsync();

        if (person == null)
            return null;

        return person.ToDto();
    }
}