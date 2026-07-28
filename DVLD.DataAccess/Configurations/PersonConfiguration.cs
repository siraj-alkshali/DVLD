using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.PersonID);

        builder.HasOne(p => p.Gender)
        .WithMany(g => g.People)
        .HasForeignKey(p => p.GenderID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_People_Genders");

        builder.HasOne(p => p.NationalityCountry)
        .WithMany(c => c.People)
        .HasForeignKey(p => p.NationalityCountryID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_People_Countries");

        builder.HasIndex(p => p.NationalNo, "UQ_People_NationalNo")
        .IsUnique();

        builder.Property(p => p.NationalNo).HasMaxLength(20).IsRequired();

        builder.Property(p => p.FirstName).HasMaxLength(20).IsRequired();

        builder.Property(p => p.SecondName).HasMaxLength(20).IsRequired();

        builder.Property(p => p.ThirdName).HasMaxLength(20);

        builder.Property(p => p.DateOfBirth).HasColumnType("DATE").IsRequired();

        builder.Property(p => p.Address).HasMaxLength(500).IsRequired();

        builder.Property(p => p.Phone).HasMaxLength(20).IsRequired();

        builder.Property(p => p.Email).HasMaxLength(50);

        builder.Property(p => p.ImagePath).HasMaxLength(250);
    }
}