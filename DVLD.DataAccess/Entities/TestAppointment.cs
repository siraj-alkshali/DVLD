namespace DVLD.DataAccess.Entities;

public class TestAppointment
{
    public int TestAppointmentID { get; set; }
    public int TestTypeID { get; set; }
    public int LocalDrivingLicenseApplicationID { get; set; }
    public DateTime AppointmentTime { get; set; }
    public decimal PaidFees { get; set; }
    public int CreatedByUserID { get; set; }
    public bool IsLocked { get; set; }
    public int? RetakeTestApplicationID { get; set; }

    // Navigation properties

    public virtual TestType TestType { get; set; } = null!;
    public virtual LocalDrivingLicenseApplication LocalDrivingLicenseApplication { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual Application? RetakeApplication { get; set; }
    public virtual Test? Test { get; set; }
}