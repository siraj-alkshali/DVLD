using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class DetainedLicenseConfiguration : IEntityTypeConfiguration<DetainedLicense>
{
    public void Configure(EntityTypeBuilder<DetainedLicense> builder)
    {

        builder.HasKey(detainedLicense => detainedLicense.DetainID);

        builder.HasOne(detainedLicense => detainedLicense.License)
        .WithMany(license => license.Detentions)
        .HasForeignKey(detainedLicense => detainedLicense.LicenseID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_DetainedLicenses_Licenses");

        builder.HasOne(detainedLicense => detainedLicense.CreatedByUser)
        .WithMany(user => user.DetainedLicensesCreated)
        .HasForeignKey(detainedLicense => detainedLicense.CreatedByUserID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_DetainedLicenses_CreatingUser");

        builder.HasOne(detainedLicense => detainedLicense.ReleasedByUser)
        .WithMany(user => user.DetainedLicensesReleased)
        .HasForeignKey(detainedLicense => detainedLicense.ReleasedByUserID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_DetainedLicenses_ReleasingUser");

        builder.HasOne(detainedLicense => detainedLicense.ReleasedByApplication)
        .WithOne(baseApp => baseApp.ReleasedDetention)
        .HasForeignKey<DetainedLicense>(detainedLicense => detainedLicense.ReleaseApplicationID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_DetainedLicenses_Applications");

        builder.HasIndex(detainedLicense => detainedLicense.ReleaseApplicationID, "UQ_DetainedLicenses_ReleaseApplicationID")
        .IsUnique();

        builder.Property(detainedLicense => detainedLicense.DetainDate).HasColumnType("DATE");

        builder.Property(detainedLicense => detainedLicense.FineFees).HasPrecision(10, 2);

        builder.Property(detainedLicense => detainedLicense.ReleaseDate).HasColumnType("DATE");
    }
}