using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class LocalDrivingLicenseApplicationConfiguration : IEntityTypeConfiguration<LocalDrivingLicenseApplication>
{
    public void Configure(EntityTypeBuilder<LocalDrivingLicenseApplication> builder)
    {

        builder.HasKey(localApp => localApp.LocalDrivingLicenseApplicationID);

        builder.HasOne(localApp => localApp.LicenseClass)
        .WithMany(licenseClass => licenseClass.LocalDrivingLicenseApplications)
        .HasForeignKey(localApp => localApp.LicenseClassID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_LocalDrivingLicenseApplications_LicenseClasses");

        builder.HasOne(localApp => localApp.BaseApplication)
        .WithOne(baseApp => baseApp.LocalDrivingLicenseApplication)
        .HasForeignKey<LocalDrivingLicenseApplication>(localApp => localApp.ApplicationID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_LocalDrivingLicenseApplications_Applications");

        builder.HasIndex(localApp => localApp.ApplicationID, "UQ_LocalDrivingLicenseApplications_ApplicationID")
        .IsUnique();
    }
}