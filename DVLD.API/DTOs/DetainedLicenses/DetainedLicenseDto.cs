namespace DVLD.API.DTOs.DetainedLicenses;

public class DetainedLicenseDto
{
    public int DetainID { get; set; }
    public string FullName { get; set; }
    public string NationalNo { get; set; }
    public string Phone { get; set; }
    public decimal FineFees { get; set; }
    public DateOnly DetainDate { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public string? ReleasedByUserName { get; set; }

    public DetainedLicenseDto(int detainId, string fullName, string nationalNo, string phone, decimal fineFees, DateOnly detainDate, DateOnly? releaseDate, string? releasedByUserName)
    {
        DetainID = detainId;
        FullName = fullName;
        NationalNo = nationalNo;
        Phone = phone;
        FineFees = fineFees;
        DetainDate = detainDate;
        ReleaseDate = releaseDate;
        ReleasedByUserName = releasedByUserName;
    }
}