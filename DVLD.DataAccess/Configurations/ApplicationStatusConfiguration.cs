using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class ApplicationStatusConfiguration : IEntityTypeConfiguration<ApplicationStatus>
{
    public void Configure(EntityTypeBuilder<ApplicationStatus> builder)
    {
        builder.HasKey(appStatus => appStatus.ApplicationStatusID);

        builder.Property(appStatus => appStatus.StatusName);
    }
}