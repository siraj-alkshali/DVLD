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

    public PersonService(DVLDContext context)
    {
        _context = context;
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
                        .Include(p => p.Gender)
                        .Include(p => p.NationalityCountry)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();

        if (person == null)
            return null;

        return person.ToDto();
    }

    public async Task<ServiceResult<PersonDto>> CreatePersonAsync(CreatePersonDto createPersonDto)
    {

        List<string> errors = new List<string>();

        bool existingNationalNo = await _context.People.AnyAsync(p => p.NationalNo == createPersonDto.NationalNo);

        if (existingNationalNo)
        {
            errors.Add("A person with this national number already exists");
        }

        bool genderExists = await _context.Genders
        .AnyAsync(g => g.GenderID == createPersonDto.GenderID);

        if (!genderExists)
        {
            errors.Add("The selected gender does not exist");
        }

        bool countryExists = await _context.Countries.AnyAsync(c => c.CountryID == createPersonDto.NationalityCountryID);

        if (!countryExists)
        {
            errors.Add("The selected nationality country does not exist");
        }

        if (errors.Count > 0)
        {
            return ServiceResult<PersonDto>.Failure(errors, FailureType.Conflict);
        }

        Person person = new Person
        {
            NationalNo = createPersonDto.NationalNo,
            FirstName = createPersonDto.FirstName,
            SecondName = createPersonDto.SecondName,
            ThirdName = createPersonDto.ThirdName,
            LastName = createPersonDto.LastName,
            DateOfBirth = createPersonDto.DateOfBirth,
            GenderID = createPersonDto.GenderID,
            Address = createPersonDto.Address,
            Phone = createPersonDto.Phone,
            Email = createPersonDto.Email,
            NationalityCountryID = createPersonDto.NationalityCountryID,
        };

        await _context.People.AddAsync(person);

        await _context.SaveChangesAsync();

        Person data = await _context.People.Where(p => p.PersonID == person.PersonID)
                        .Include(p => p.Gender)
                        .Include(p => p.NationalityCountry)
                        .AsNoTracking()
                        .SingleAsync();

        return ServiceResult<PersonDto>.Success(data.ToDto());
    }

    public async Task<bool> UpdatePersonAsync(int id, UpdatePersonDto updatePersonDto)
    {

        Person? person = await _context.People
            .FindAsync(id);

        if (person == null)
        {
            return false;
        }

        person.NationalNo = updatePersonDto.NationalNo;
        person.FirstName = updatePersonDto.FirstName;
        person.SecondName = updatePersonDto.SecondName;
        person.ThirdName = updatePersonDto.ThirdName;
        person.LastName = updatePersonDto.LastName;
        person.DateOfBirth = updatePersonDto.DateOfBirth;
        person.GenderID = updatePersonDto.GenderID;
        person.Address = updatePersonDto.Address;
        person.Phone = updatePersonDto.Phone;
        person.Email = updatePersonDto.Email;
        person.NationalityCountryID = updatePersonDto.NationalityCountryID;

        await _context.SaveChangesAsync();

        return true;
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
                        .Include(p => p.Gender)
                        .Include(p => p.NationalityCountry)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();

        if (person == null)
            return null;

        return person.ToDto();
    }
}