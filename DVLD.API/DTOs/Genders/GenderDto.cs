namespace DVLD.API.DTOs.Countries;

public class GenderDto
{

    public GenderDto(int genderID, string genderName)
    {
        GenderID = genderID;
        GenderName = genderName;
    }

    public int GenderID { get; set; }
    public string GenderName { get; set; } = null!;
}