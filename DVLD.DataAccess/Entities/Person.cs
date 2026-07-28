namespace DVLD.DataAccess.Entities;

public class Person
{
    public int PersonID { get; set; }
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
    public string? ImagePath { get; set; }

    // Navigation properties

    public virtual Gender Gender { get; set; } = null!;
    public virtual Country NationalityCountry { get; set; } = null!;
}