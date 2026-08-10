namespace DVLD.API.DTOs;

public class DriverDto
{
    public int DriverID { get; set; }
    public string FullName { get; set; }
    public string NationalNo { get; set; }
    public string Phone { get; set; }
    public string CreatedByUserName { get; set; }

    public DriverDto(int driverId, string fullName, string nationalNo, string phone, string createdByUserName)
    {
        DriverID = driverId;
        FullName = fullName;
        NationalNo = nationalNo;
        Phone = phone;
        CreatedByUserName = createdByUserName;
    }
}