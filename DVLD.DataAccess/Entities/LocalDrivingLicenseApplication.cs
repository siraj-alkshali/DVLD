using System.ComponentModel;

namespace DVLD.DataAccess.Entities;

public class LocalDrivingLicenseApplication
{
    public int LocalDrivingLicenseApplicationID { get; set; }
    public int ApplicationID { get; set; }
    public int LicenseClassID { get; set; }

    // Navigation properties

    public virtual Application BaseApplication { get; set; } = null!;
    public virtual LicenseClass LicenseClass { get; set; } = null!;
}