namespace DVLD.API.DTOs.People;

public class CreatePersonDto : IPersonDto
{
    public string NationalNo { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string? ThirdName { get; set; }
    public string LastName { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public int GenderID { get; set; }
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public int NationalityCountryID { get; set; }
}