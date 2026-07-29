using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.API.DTOs.People;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class PersonService : IPersonService
{
    private readonly DVLDContext _context;

    public PersonService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<int> CreatePersonAsync(CreatePersonDto dto)
    {
        Person person = new Person
        {
            NationalNo = dto.NationalNo,
            FirstName = dto.FirstName,
            SecondName = dto.SecondName,
            ThirdName = dto.ThirdName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            GenderID = dto.GenderID,
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            NationalityCountryID = dto.NationalityCountryID,
            ImagePath = dto.ImagePath
        };

        await _context.People.AddAsync(person);

        await _context.SaveChangesAsync();

        return person.PersonID;
    }

    public async Task<PersonDto?> GetPersonByIDAsync(int id)
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
    )).SingleOrDefaultAsync(p => p.PersonID == id);
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

    public async Task<PersonDto?> GetPersonByNationalNoAsync(string nationalNo)
    {
        return await _context.People
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
            ))
            .SingleOrDefaultAsync(p => p.NationalNo == nationalNo);
    }

    public async Task<bool> UpdatePersonAsync(int id, UpdatePersonDto dto)
    {
        Person? person = await _context.People
            .FindAsync(id);

        if (person == null)
        {
            return false;
        }

        person.NationalNo = dto.NationalNo;
        person.FirstName = dto.FirstName;
        person.SecondName = dto.SecondName;
        person.ThirdName = dto.ThirdName;
        person.LastName = dto.LastName;
        person.DateOfBirth = dto.DateOfBirth;
        person.GenderID = dto.GenderID;
        person.Address = dto.Address;
        person.Phone = dto.Phone;
        person.Email = dto.Email;
        person.NationalityCountryID = dto.NationalityCountryID;

        await _context.SaveChangesAsync();

        return true;
    }
}