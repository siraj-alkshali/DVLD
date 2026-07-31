namespace DVLD.API.DTOs.People;

public class PersonDto
{
    public int PersonID { get; set; }
    public string NationalNo { get; set; }
    public string FullName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string? Email { get; set; }
    public string Nationality { get; set; }
    public string? ImageUrl { get; set; }

    public PersonDto(
    int personID,
    string nationalNo,
    string fullName,
    DateOnly dateOfBirth,
    string gender,
    string address,
    string phone,
    string? email,
    string nationality,
    string? imageUrl)
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
        ImageUrl = imageUrl;
    }
}