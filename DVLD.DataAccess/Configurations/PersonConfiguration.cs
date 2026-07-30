using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(person => person.PersonID);

        builder.HasOne(person => person.Gender)
        .WithMany(gender => gender.People)
        .HasForeignKey(person => person.GenderID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_People_Genders");

        builder.HasOne(person => person.NationalityCountry)
        .WithMany(country => country.People)
        .HasForeignKey(person => person.NationalityCountryID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_People_Countries");

        builder.HasIndex(person => person.NationalNo, "UQ_People_NationalNo")
        .IsUnique();

        builder.Property(person => person.NationalNo).HasMaxLength(10).IsRequired();

        builder.Property(person => person.FirstName).HasMaxLength(20).IsRequired();

        builder.Property(person => person.SecondName).HasMaxLength(20).IsRequired();

        builder.Property(person => person.ThirdName).HasMaxLength(20);

        builder.Property(person => person.DateOfBirth).HasColumnType("DATE").IsRequired();

        builder.Property(person => person.Address).HasMaxLength(500).IsRequired();

        builder.Property(person => person.Phone).HasMaxLength(20).IsRequired();

        builder.Property(person => person.Email).HasMaxLength(50);

        builder.Property(person => person.ImagePath).HasMaxLength(250);
    }
}