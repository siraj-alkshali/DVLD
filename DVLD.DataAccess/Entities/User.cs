namespace DVLD.DataAccess.Entities;

public class User
{
    public int UserID { get; set; }
    public int PersonID { get; set; }
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsActive { get; set; }

    // Navigation properties

    public virtual Person Person { get; set; } = null!;
    public virtual ICollection<Application> ApplicationsCreated { get; set; } = new List<Application>();
    public virtual ICollection<Driver> DriversCreated { get; set; } = new List<Driver>();
    public virtual ICollection<License> LicensesCreated { get; set; } = new List<License>();
    public virtual ICollection<TestAppointment> TestAppointmentsCreated { get; set; } = new List<TestAppointment>();
}