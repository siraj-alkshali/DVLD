namespace DVLD.API.DTOs.ApplicationTypes;

public class ApplicationTypeDto
{
    public ApplicationTypeDto(int applicationTypeID, string applicationTypeTitle, decimal applicationFees)
    {
        ApplicationTypeID = applicationTypeID;
        ApplicationTypeTitle = applicationTypeTitle;
        ApplicationFees = applicationFees;
    }

    public int ApplicationTypeID { get; set; }
    public string ApplicationTypeTitle { get; set; }
    public decimal ApplicationFees { get; set; }
}