using Microsoft.EntityFrameworkCore;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Data;

public class DVLDContext : DbContext
{
    public DVLDContext(DbContextOptions<DVLDContext> options) : base(options)
    {
    }

    public virtual DbSet<Person> People { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<Gender> Genders { get; set; }
    public virtual DbSet<LicenseClass> LicenseClasses { get; set; }
    public virtual DbSet<ApplicationType> ApplicationTypes { get; set; }
    public virtual DbSet<LicenseIssueReason> LicenseIssueReasons { get; set; }
    public virtual DbSet<ApplicationStatus> ApplicationStatuses { get; set; }
    public virtual DbSet<TestType> TestTypes { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Application> Applications { get; set; }
    public virtual DbSet<LocalDrivingLicenseApplication> LocalDrivingLicenseApplications { get; set; }
    public virtual DbSet<Driver> Drivers { get; set; }
    public virtual DbSet<License> Licenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DVLDContext).Assembly);
    }
}