namespace DVLD.DataAccess.Entities;

public class Application
{
    public int ApplicationID { get; set; }
    public int ApplicantPersonID { get; set; }
    public DateOnly ApplicationDate { get; set; }
    public int ApplicationTypeID { get; set; }
    public int ApplicationStatusID { get; set; }
    public DateOnly LastStatusDate { get; set; }
    public decimal PaidFees { get; set; }
    public int CreatedByUserID { get; set; }

    // Navigation properties

    public virtual Person ApplicantPerson { get; set; } = null!;
    public virtual LocalDrivingLicenseApplication? LocalDrivingLicenseApplication { get; set; }
    public virtual ApplicationType ApplicationType { get; set; } = null!;
    public virtual ApplicationStatus ApplicationStatus { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;

}