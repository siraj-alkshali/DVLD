using Microsoft.EntityFrameworkCore;
using DVLD.DataAccess.Entities;
using DVLD.DataAccess.Configurations;
using System.ComponentModel;

namespace DVLD.DataAccess.Data;

public class DVLDContext : DbContext
{
    public DVLDContext(DbContextOptions<DVLDContext> options) : base(options)
    {
    }

    public virtual DbSet<Person> People { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<Gender> Genders { get; set; }
    public virtual DbSet<License> LicenseClasses { get; set; }
    public virtual DbSet<ApplicationType> ApplicationTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DVLDContext).Assembly);
    }
}