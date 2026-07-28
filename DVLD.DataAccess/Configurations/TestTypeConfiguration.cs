using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class TestTypeConfiguration : IEntityTypeConfiguration<TestType>
{
    public void Configure(EntityTypeBuilder<TestType> builder)
    {

        builder.HasKey(testType => testType.TestTypeID);

        builder.Property(testType => testType.TestTypeTitle).HasMaxLength(100).IsRequired();

        builder.Property(testType => testType.TestTypeDescription).HasMaxLength(500);

        builder.Property(testType => testType.TestTypeFees).HasPrecision(10, 2);

    }
}