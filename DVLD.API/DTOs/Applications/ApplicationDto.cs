namespace DVLD.API.DTOs.Applications;

public class ApplicationDto
{
    public int ApplicationID { get; set; }
    public string ApplicantName { get; set; } = null!;
    public string NationalNo { get; set; } = null!;
    public DateOnly ApplicationDate { get; set; }
    public string ApplicationType { get; set; } = null!;
    public string ApplicationStatus { get; set; } = null!;
    public string CreatedByUserName { get; set; } = null!;

    public ApplicationDto(int applicationID, string applicantName, string nationalNo, DateOnly applicationDate, string applicationType, string applicationStatus, string createdByUserName)
    {
        ApplicationID = applicationID;
        ApplicantName = applicantName;
        NationalNo = nationalNo;
        ApplicationDate = applicationDate;
        ApplicationType = applicationType;
        ApplicationStatus = applicationStatus;
        CreatedByUserName = createdByUserName;
    }
}