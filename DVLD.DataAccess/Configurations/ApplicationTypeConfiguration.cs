using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class ApplicationTypeConfiguration : IEntityTypeConfiguration<ApplicationType>
{
    public void Configure(EntityTypeBuilder<ApplicationType> builder)
    {
        builder.HasKey(at => at.ApplicationTypeID);

        builder.Property(at => at.ApplicationTypeTitle).HasMaxLength(50).IsRequired();

        builder.Property(at => at.ApplicationFees).HasPrecision(10, 2);
    }
}