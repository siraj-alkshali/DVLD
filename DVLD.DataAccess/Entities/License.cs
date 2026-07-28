namespace DVLD.DataAccess.Entities;

public class License
{
    public int LicenseID { get; set; }
    public int ApplicationID { get; set; }
    public int DriverID { get; set; }
    public int LicenseClassID { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public string? Notes { get; set; }
    public decimal PaidFees { get; set; }
    public bool IsActive { get; set; }
    public int IssueReasonID { get; set; }
    public int CreatedByUserID { get; set; }

    // Navigation properties

    public virtual Application Application { get; set; } = null!;
    public virtual Driver Driver { get; set; } = null!;
    public virtual LicenseClass LicenseClass { get; set; } = null!;
    public virtual LicenseIssueReason LicenseIssueReason { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual ICollection<InternationalLicense> InternationalLicensesHistory { get; set; } = new List<InternationalLicense>();
    public virtual ICollection<DetainedLicense> Detentions { get; set; } = new List<DetainedLicense>();

}