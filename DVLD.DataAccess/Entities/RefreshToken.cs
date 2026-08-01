namespace DVLD.DataAccess.Entities;

public class RefreshToken
{
    public int RefreshTokenID { get; set; }
    public int UserID { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime? RevokedDate { get; set; }
    public string? ReplacedByTokenHash { get; set; }

    // Navigation properties

    public virtual User User { get; set; } = null!;
}