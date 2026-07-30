using DVLD.API.DTOs.People;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Mappings.People;

public static class PersonMappingExtensions
{
    public static PersonDto ToDto(this Person person)
    {
        return new PersonDto(
            person.PersonID,
            person.NationalNo,
            $"{person.FirstName} {person.SecondName} {person.ThirdName} {person.LastName}",
            person.DateOfBirth,
            person.Gender.GenderName,
            person.Address,
            person.Phone,
            person.Email,
            person.NationalityCountry.CountryName
        );
    }

    public static Person ToEntity(this CreatePersonDto dto)
    {
        return new Person
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
            NationalityCountryID = dto.NationalityCountryID
        };
    }

    public static void UpdateFromDto(this Person person, UpdatePersonDto dto)
    {
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
    }
}