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

    public PersonDto(
    int personID,
    string nationalNo,
    string fullName,
    DateOnly dateOfBirth,
    string gender,
    string address,
    string phone,
    string? email,
    string nationality)
    {
        PersonID = personID;
        NationalNo = nationalNo;
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Address = address;
        Phone = phone;
        Email = email;
        Nationality = nationality;
    }
}