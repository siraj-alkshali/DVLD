namespace DVLD.API.DTOs.ApplicationStatuses;

public class ApplicationStatusDto
{
    public ApplicationStatusDto(int applicationStatusID, string statusName)
    {
        ApplicationStatusID = applicationStatusID;
        StatusName = statusName;
    }

    public int ApplicationStatusID { get; set; }
    public string StatusName { get; set; }
}