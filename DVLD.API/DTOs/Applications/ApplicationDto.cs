namespace DVLD.API.DTOs.Applications;

public class ApplicationDto
{
    public int ApplicationID { get; set; }
    public string ApplicantName { get; set; }
    public string NationalNo { get; set; }
    public DateOnly ApplicationDate { get; set; }
    public string ApplicationType { get; set; }
    public string ApplicationStatus { get; set; }
    public string CreatedByUserName { get; set; }

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