namespace DVLD.DataAccess.Entities;

public class InternationalLicense
{
    public int InternationalLicenseID { get; set; }
    public int ApplicationID { get; set; }
    public int DriverID { get; set; }
    public int IssuedUsingLocalLicenseID { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public int CreatedByUserID { get; set; }

    // Navigation properties

    public virtual Application BaseApplication { get; set; } = null!;
    public virtual Driver Driver { get; set; } = null!;
    public virtual License IssuedUsingLocalLicense { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
}