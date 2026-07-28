using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class TestAppointmentConfiguration : IEntityTypeConfiguration<TestAppointment>
{
    public void Configure(EntityTypeBuilder<TestAppointment> builder)
    {

        builder.HasKey(testApp => testApp.TestAppointmentID);

        builder.HasOne(testApp => testApp.TestType)
        .WithMany(testType => testType.TestAppointments)
        .HasForeignKey(testApp => testApp.TestTypeID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_TestAppointments_TestTypes");

        builder.HasOne(testApp => testApp.LocalDrivingLicenseApplication)
        .WithMany(localApp => localApp.TestAppointments)
        .HasForeignKey(testApp => testApp.LocalDrivingLicenseApplicationID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_TestAppointments_LocalDrivingLicenseApplications");

        builder.HasOne(testApp => testApp.CreatedByUser)
        .WithMany(user => user.TestAppointmentsCreated)
        .HasForeignKey(testApp => testApp.CreatedByUserID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_TestAppointments_Users");

        builder.HasOne(testApp => testApp.RetakeApplication)
        .WithOne(retakeApp => retakeApp.TestRetake)
        .HasForeignKey<TestAppointment>(testApp => testApp.RetakeTestApplicationID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_TestAppointments_Applications");

        builder.HasIndex(testApp => testApp.RetakeTestApplicationID, "UQ_TestAppointments_Applications")
        .IsUnique();

        builder.Property(testApp => testApp.PaidFees).HasPrecision(10, 2);
    }
}