namespace DVLD.DataAccess.Entities;

public class Role
{
    public int RoleID { get; set; }
    public string RoleTitle { get; set; } = null!;

    // Navigation properties

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}