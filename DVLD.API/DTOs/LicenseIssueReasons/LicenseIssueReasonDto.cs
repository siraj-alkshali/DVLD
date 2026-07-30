namespace DVLD.API.DTOs.LicenseIssueReasons;

public class LicenseIssueReasonDto
{
    public LicenseIssueReasonDto(int issueReasonID, string issueReasonName)
    {
        IssueReasonID = issueReasonID;
        IssueReasonName = issueReasonName;
    }

    public int IssueReasonID { get; set; }
    public string IssueReasonName { get; set; }
}