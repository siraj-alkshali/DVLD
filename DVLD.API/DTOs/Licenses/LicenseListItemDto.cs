namespace DVLD.API.DTOs.Licenses;

public class LicenseListItemDto
{
    public int LicenseID { get; set; }
    public string LicenseClass { get; set; }
    public string IssueReason { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public bool IsActive { get; set; }

    public LicenseListItemDto(int licenseId, string licenseClass, string issueReason, DateOnly issueDate, DateOnly expirationDate, bool isActive)
    {
        LicenseID = licenseId;
        LicenseClass = licenseClass;
        IssueReason = issueReason;
        IssueDate = issueDate;
        ExpirationDate = expirationDate;
        IsActive = isActive;
    }
}