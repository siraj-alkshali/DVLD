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
}