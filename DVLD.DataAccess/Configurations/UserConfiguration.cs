using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.UserID);

        builder.HasOne(user => user.Person)
        .WithOne(person => person.User)
        .HasForeignKey<User>(user => user.PersonID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Users_People");

        builder.HasOne(user => user.Role)
        .WithMany(role => role.Users)
        .HasForeignKey(user => user.RoleID)
        .OnDelete(DeleteBehavior.Restrict)
        .HasConstraintName("FK_Users_Roles");

        builder.HasIndex(user => user.PersonID, "UQ_Users_PersonID").IsUnique();

        builder.HasIndex(user => user.UserName, "UQ_Users_UserName").IsUnique();

        builder.Property(user => user.UserName).HasMaxLength(50).IsRequired();

        builder.Property(user => user.PasswordHash).HasMaxLength(255).IsRequired();
    }
}