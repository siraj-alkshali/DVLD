using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {

        builder.HasKey(test => test.TestID);

        builder.HasOne(test => test.CreatedByUser)
        .WithMany(user => user.TestsCreated)
        .HasForeignKey(test => test.CreatedByUserID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Tests_Users");

        builder.HasOne(test => test.TestAppointment)
        .WithOne(testApp => testApp.Test)
        .HasForeignKey<Test>(test => test.TestAppointmentID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Tests_TestAppointments");

        builder.HasIndex(test => test.TestAppointmentID, "UQ_Tests_TestAppointments")
        .IsUnique();

        builder.Property(test => test.Notes).HasMaxLength(500);
    }
}