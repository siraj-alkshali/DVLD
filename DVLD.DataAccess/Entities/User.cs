namespace DVLD.DataAccess.Entities;

public class User
{
    public int UserID { get; set; }
    public int PersonID { get; set; }
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsActive { get; set; }
    public int RoleID { get; set; }

    // Navigation properties

    public virtual Person Person { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<Application> ApplicationsCreated { get; set; } = new List<Application>();
    public virtual ICollection<Driver> DriversCreated { get; set; } = new List<Driver>();
    public virtual ICollection<License> LicensesIssued { get; set; } = new List<License>();
    public virtual ICollection<TestAppointment> TestAppointmentsCreated { get; set; } = new List<TestAppointment>();
    public virtual ICollection<Test> TestsCreated { get; set; } = new List<Test>();
    public virtual ICollection<InternationalLicense> InternationalLicensesIssued { get; set; } = new List<InternationalLicense>();
    public virtual ICollection<DetainedLicense> DetainedLicensesCreated { get; set; } = new List<DetainedLicense>();
    public virtual ICollection<DetainedLicense> DetainedLicensesReleased { get; set; } = new List<DetainedLicense>();

}