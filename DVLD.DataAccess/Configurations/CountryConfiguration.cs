using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(c => c.CountryID);

        // builder.HasMany(c => c.People)
        // .WithOne(p => p.NationalityCountry)
        // .HasForeignKey(p => p.NationalityCountryID)
        // .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.CountryName).HasMaxLength(50).IsRequired();
    }
}