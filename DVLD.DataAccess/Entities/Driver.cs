namespace DVLD.DataAccess.Entities;

public class Driver
{
    public int DriverID { get; set; }
    public int PersonID { get; set; }
    public int CreatedByUserID { get; set; }
    public DateOnly CreatedDate { get; set; }

    // Navigation properties

    public virtual Person Person { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual ICollection<License> Licenses { get; set; } = new List<License>();
}