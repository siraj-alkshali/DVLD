using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class InternationalLicenseConfiguration : IEntityTypeConfiguration<InternationalLicense>
{
    public void Configure(EntityTypeBuilder<InternationalLicense> builder)
    {

        builder.HasKey(intlLicense => intlLicense.InternationalLicenseID);

        builder.HasOne(intlLicense => intlLicense.Driver)
        .WithMany(driver => driver.InternationalLicenses)
        .HasForeignKey(intlLicense => intlLicense.DriverID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_InternationalLicenses_Drivers");

        builder.HasOne(intlLicense => intlLicense.IssuedUsingLocalLicense)
        .WithMany(localLicense => localLicense.InternationalLicensesHistory)
        .HasForeignKey(intlLicense => intlLicense.IssuedUsingLocalLicenseID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_InternationalLicenses_Licenses");

        builder.HasOne(intlLicense => intlLicense.CreatedByUser)
        .WithMany(user => user.InternationalLicensesIssued)
        .HasForeignKey(intlLicense => intlLicense.CreatedByUserID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_InternationalLicenses_Users");

        builder.HasOne(intlLicense => intlLicense.BaseApplication)
        .WithOne(baseApp => baseApp.InternationalLicenseApplication)
        .HasForeignKey<InternationalLicense>(intlLicense => intlLicense.ApplicationID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_InternationalLicenses_Applications");

        builder.HasIndex(intlLicense => intlLicense.ApplicationID, "UQ_InternationalLicenses_ApplicationID")
        .IsUnique();

        builder.Property(intlLicense => intlLicense.IssueDate).HasColumnType("DATE");

        builder.Property(intlLicense => intlLicense.ExpirationDate).HasColumnType("DATE");
    }
}