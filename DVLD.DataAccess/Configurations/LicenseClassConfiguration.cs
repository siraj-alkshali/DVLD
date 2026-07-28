using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class LicenseClassConfiguration : IEntityTypeConfiguration<LicenseClass>
{
    public void Configure(EntityTypeBuilder<LicenseClass> builder)
    {
        builder.HasKey(licenseClass => licenseClass.LicenseClassID);

        builder.Property(licenseClass => licenseClass.ClassName).HasMaxLength(50).IsRequired();

        builder.Property(licenseClass => licenseClass.ClassDescription).HasMaxLength(500).IsRequired();

        builder.Property(licenseClass => licenseClass.ClassFees).HasPrecision(10, 2);

    }
}