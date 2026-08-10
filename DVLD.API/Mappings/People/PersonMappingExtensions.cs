using DVLD.API.DTOs.People;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Mappings.People;

public static class PersonMappingExtensions
{
    public static PersonDto ToDto(this Person person, string? imageUrl)
    {
        return new PersonDto(
            person.PersonID,
            person.NationalNo,
            $"{person.FirstName} {person.SecondName} {person.LastName}",
            person.DateOfBirth,
            person.Gender.GenderName,
            person.Address,
            person.Phone,
            person.Email,
            person.NationalityCountry.CountryName,
            imageUrl
        );
    }

    public static Person ToEntity(this CreatePersonDto dto, Gender gender, Country country)
    {
        return new Person
        {
            NationalNo = dto.NationalNo,
            FirstName = dto.FirstName,
            SecondName = dto.SecondName,
            ThirdName = dto.ThirdName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            GenderID = gender.GenderID,
            Gender = gender,
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            NationalityCountryID = country.CountryID,
            NationalityCountry = country
        };
    }

    public static void UpdateFromDto(this Person person, UpdatePersonDto dto, Gender updatedGender, Country updatedCountry)
    {
        person.NationalNo = dto.NationalNo;
        person.FirstName = dto.FirstName;
        person.SecondName = dto.SecondName;
        person.ThirdName = dto.ThirdName;
        person.LastName = dto.LastName;
        person.DateOfBirth = dto.DateOfBirth;
        person.GenderID = dto.GenderID;
        person.Gender = updatedGender;
        person.Address = dto.Address;
        person.Phone = dto.Phone;
        person.Email = dto.Email;
        person.NationalityCountryID = dto.NationalityCountryID;
        person.NationalityCountry = updatedCountry;
    }
}