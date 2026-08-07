namespace DVLD.API.DTOs;

public class InternationalLicenseDto
{
    public int InternationalLicenseID { get; set; }
    public string FullName { get; set; }
    public string NationalNo { get; set; }
    public string Phone { get; set; }
    public string LicenseClass { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public bool IsActive { get; set; }

    public InternationalLicenseDto(int internationalLicenseId, string fullName, string nationalNo, string phone, string licenseClass, DateOnly issueDate, DateOnly expirationDate, bool isActive)
    {
        InternationalLicenseID = internationalLicenseId;
        FullName = fullName;
        NationalNo = nationalNo;
        Phone = phone;
        LicenseClass = licenseClass;
        IssueDate = issueDate;
        ExpirationDate = expirationDate;
        IsActive = isActive;
    }

}