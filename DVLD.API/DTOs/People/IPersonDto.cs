namespace DVLD.API.DTOs.People;

public interface IPersonDto
{
    string NationalNo { get; set; }
    string FirstName { get; set; }
    string SecondName { get; set; }
    string? ThirdName { get; set; }
    string LastName { get; set; }
    DateOnly DateOfBirth { get; set; }
    int GenderID { get; set; }
    string Address { get; set; }
    string Phone { get; set; }
    string? Email { get; set; }
    int NationalityCountryID { get; set; }
}