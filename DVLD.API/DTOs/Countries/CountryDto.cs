namespace DVLD.API.DTOs.Countries;

public class CountryDto
{

    public CountryDto(int countryID, string countryName)
    {
        CountryID = countryID;
        CountryName = countryName;
    }

    public int CountryID { get; set; }
    public string CountryName { get; set; }
}