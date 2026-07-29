namespace DVLD.API.DTOs.People;

public class PersonDto
{
    public int PersonID { get; set; }
    public string NationalNo { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string Nationality { get; set; } = null!;
    public string? ImagePath { get; set; }
}