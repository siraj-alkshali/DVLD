using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {

        builder.HasKey(app => app.ApplicationID);

        builder.HasOne(app => app.ApplicantPerson)
        .WithMany(person => person.Applications)
        .HasForeignKey(app => app.ApplicantPersonID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Applications_People");

        builder.HasOne(app => app.ApplicationType)
        .WithMany(appType => appType.Applications)
        .HasForeignKey(app => app.ApplicationTypeID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Applications_ApplicationTypes");

        builder.HasOne(app => app.ApplicationStatus)
        .WithMany(appStatus => appStatus.Applications)
        .HasForeignKey(app => app.ApplicationStatusID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Applications_ApplicationStatuses");

        builder.HasOne(app => app.CreatedByUser)
        .WithMany(user => user.ApplicationsCreated)
        .HasForeignKey(app => app.CreatedByUserID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Applications_Users");

        builder.Property(app => app.ApplicationDate).HasColumnType("DATE");

        builder.Property(app => app.LastStatusDate).HasColumnType("DATE");

        builder.Property(app => app.PaidFees).HasPrecision(10, 2);
    }
}