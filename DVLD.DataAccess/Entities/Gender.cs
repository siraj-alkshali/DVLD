namespace DVLD.DataAccess.Entities;

public partial class Gender
{
    public int GenderID { get; set; }
    public string GenderName { get; set; } = null!;
    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}