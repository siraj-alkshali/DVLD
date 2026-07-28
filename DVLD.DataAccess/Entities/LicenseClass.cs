namespace DVLD.DataAccess.Entities;

public class LicenseClass
{
    public int LicenseClassID { get; set; }
    public string ClassName { get; set; } = null!;
    public string ClassDescription { get; set; } = null!;
    public byte MinimumAllowedAge { get; set; }
    public byte DefaultValidityLength { get; set; }
    public decimal ClassFees { get; set; }

    // Navigation properties

    public virtual ICollection<LocalDrivingLicenseApplication> LocalDrivingLicenseApplications { get; set; } = null!;
}