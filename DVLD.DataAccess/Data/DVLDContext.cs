using Microsoft.EntityFrameworkCore;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Data;

public partial class DVLDContext : DbContext
{
    public DVLDContext(DbContextOptions<DVLDContext> options) : base(options)
    {
    }

    public virtual DbSet<Person> People { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<Gender> Genders { get; set; }
}