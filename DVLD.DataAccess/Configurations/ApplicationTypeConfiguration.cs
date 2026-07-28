using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class ApplicationTypeConfiguration : IEntityTypeConfiguration<ApplicationType>
{
    public void Configure(EntityTypeBuilder<ApplicationType> builder)
    {
        builder.HasKey(appType => appType.ApplicationTypeID);

        builder.Property(appType => appType.ApplicationTypeTitle).HasMaxLength(50).IsRequired();

        builder.Property(appType => appType.ApplicationFees).HasPrecision(10, 2);
    }
}