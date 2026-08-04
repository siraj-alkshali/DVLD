namespace DVLD.API.DTOs.Licenses;

public class LicenseDto
{
    public int LicenseID { get; set; }
    public string FullName { get; set; }
    public string NationalNo { get; set; }
    public string Phone { get; set; }
    public string LicenseClass { get; set; }
    public string IssueReason { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public bool IsActive { get; set; }

    public LicenseDto(int licenseId, string fullName, string nationalNo, string phone, string licenseClass, string issueReason, DateOnly issueDate, DateOnly expirationDate, bool isActive)
    {
        LicenseID = licenseId;
        FullName = fullName;
        NationalNo = nationalNo;
        Phone = phone;
        LicenseClass = licenseClass;
        IssueReason = issueReason;
        IssueDate = issueDate;
        ExpirationDate = expirationDate;
        IsActive = isActive;
    }
}