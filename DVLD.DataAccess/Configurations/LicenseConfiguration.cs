using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {

        builder.HasKey(license => license.LicenseID);

        builder.HasOne(license => license.Application)
        .WithOne(app => app.License)
        .HasForeignKey<License>(license => license.ApplicationID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Licenses_Applications");

        builder.HasOne(license => license.Driver)
        .WithMany(driver => driver.Licenses)
        .HasForeignKey(license => license.DriverID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Licenses_Drivers");

        builder.HasOne(license => license.LicenseClass)
        .WithMany(licenseClass => licenseClass.Licenses)
        .HasForeignKey(license => license.LicenseClassID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Licenses_LicenseClasses");

        builder.HasOne(license => license.LicenseIssueReason)
        .WithMany(licenseIssueReason => licenseIssueReason.Licenses)
        .HasForeignKey(license => license.IssueReasonID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Licenses_LicenseIssueReasons");

        builder.HasOne(license => license.CreatedByUser)
        .WithMany(user => user.LicensesCreated)
        .HasForeignKey(license => license.CreatedByUserID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Licenses_Users");

        builder.Property(license => license.Notes).HasMaxLength(500);

        builder.HasIndex(license => license.ApplicationID, "UQ_Licenses_ApplicationID")
        .IsUnique();

        builder.Property(license => license.IssueDate).HasColumnType("DATE");

        builder.Property(license => license.ExpirationDate).HasColumnType("DATE");

        builder.Property(license => license.PaidFees).HasPrecision(10, 2);
    }
}