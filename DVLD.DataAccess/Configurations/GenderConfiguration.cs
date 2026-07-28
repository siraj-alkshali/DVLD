using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class GenderConfiguration : IEntityTypeConfiguration<Gender>
{
    public void Configure(EntityTypeBuilder<Gender> builder)
    {
        builder.HasKey(gender => gender.GenderID);

        // builder.HasMany(g => g.People)
        // .WithOne(p => p.Gender)
        // .HasForeignKey(p => p.GenderID)
        // .OnDelete(DeleteBehavior.Restrict);

        builder.Property(gender => gender.GenderName).HasMaxLength(20).IsRequired();

    }
}