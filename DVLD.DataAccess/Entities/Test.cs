namespace DVLD.DataAccess.Entities;

public class Test
{
    public int TestID { get; set; }
    public int TestAppointmentID { get; set; }
    public bool Passed { get; set; }
    public string? Notes { get; set; }
    public int CreatedByUserID { get; set; }

    // Navigation properties

    public virtual TestAppointment TestAppointment { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
}