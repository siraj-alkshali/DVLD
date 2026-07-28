namespace DVLD.DataAccess.Entities;

public partial class Person
{
    public int PersonID { get; set; }
    public string NationalNo { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string? ThirdName { get; set; }
    public string LastName { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public virtual Gender GenderID { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public virtual Country NationalityCountryID { get; set; } = null!;
    public string? ImagePath { get; set; }
}