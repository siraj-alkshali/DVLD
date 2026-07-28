using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class LicenseIssueReasonConfiguration : IEntityTypeConfiguration<LicenseIssueReason>
{
    public void Configure(EntityTypeBuilder<LicenseIssueReason> builder)
    {

        builder.HasKey(licenseIssueReason => licenseIssueReason.IssueReasonID);

        builder.Property(licenseIssueReason => licenseIssueReason.IssueReasonName).HasMaxLength(100).IsRequired();

    }
}