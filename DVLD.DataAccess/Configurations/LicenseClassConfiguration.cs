using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class LicenseClassConfiguration : IEntityTypeConfiguration<LicenseClass>
{
    public void Configure(EntityTypeBuilder<LicenseClass> builder)
    {
        builder.HasKey(lc => lc.LicenseClassID);

        builder.Property(lc => lc.ClassName).HasMaxLength(50).IsRequired();

        builder.Property(lc => lc.ClassDescription).HasMaxLength(500).IsRequired();

        builder.Property(lc => lc.ClassFees).HasPrecision(10, 2);

    }
}