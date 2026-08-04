namespace DVLD.API.DTOs.Licenses;

public class CreateLicenseDto
{
    public int LocalDrivingLicenseApplicationID { get; set; }
    public string? Notes { get; set; }
}