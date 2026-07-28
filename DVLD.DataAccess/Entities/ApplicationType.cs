namespace DVLD.DataAccess.Entities;

public class ApplicationType
{
    public int ApplicationTypeID { get; set; }
    public string ApplicationTypeTitle { get; set; } = null!;
    public decimal ApplicationFees { get; set; }

    // Navigation properties

}