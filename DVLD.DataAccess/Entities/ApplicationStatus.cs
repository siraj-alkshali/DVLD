namespace DVLD.DataAccess.Entities;

public class ApplicationStatus
{
    public int ApplicationStatusID { get; set; }
    public string StatusName { get; set; } = null!;

    // Navigation properties

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
}