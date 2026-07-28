using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {

        builder.HasKey(driver => driver.DriverID);

        builder.HasOne(driver => driver.Person)
        .WithOne(person => person.Driver)
        .HasForeignKey<Driver>(driver => driver.PersonID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Drivers_People");

        builder.HasOne(driver => driver.CreatedByUser)
        .WithMany(user => user.DriversCreated)
        .HasForeignKey(driver => driver.CreatedByUserID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Drivers_Users");

        builder.HasIndex(driver => driver.PersonID, "UQ_Drivers_PersonID")
        .IsUnique();

        builder.Property(driver => driver.CreatedDate).HasColumnType("DATE");

    }
}