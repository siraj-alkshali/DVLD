namespace DVLD.DataAccess.Entities;

public class Gender
{
    public int GenderID { get; set; }
    public string GenderName { get; set; } = null!;

    // Navigation properties

    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}