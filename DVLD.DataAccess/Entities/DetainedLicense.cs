namespace DVLD.DataAccess.Entities;

public class DetainedLicense
{
    public int DetainID { get; set; }
    public int LicenseID { get; set; }
    public DateOnly DetainDate { get; set; }
    public decimal FineFees { get; set; }
    public int CreatedByUserID { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public int? ReleasedByUserID { get; set; }
    public int? ReleaseApplicationID { get; set; }

    // Navigation properties

    public virtual License License { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual User? ReleasedByUser { get; set; }
    public virtual Application? ReleasedByApplication { get; set; }
}