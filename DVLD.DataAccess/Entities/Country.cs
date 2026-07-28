namespace DVLD.DataAccess.Entities;

public class Country
{
    public int CountryID { get; set; }
    public string CountryName { get; set; } = null!;

    // Navigation properties

    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}