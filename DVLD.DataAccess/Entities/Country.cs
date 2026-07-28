namespace DVLD.DataAccess.Entities;

public partial class Country
{
    public int CountryID { get; set; }
    public string CountryName { get; set; } = null!;
    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}