using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(role => role.RoleID);

        builder.HasIndex(role => role.RoleTitle, "UQ_Roles_RoleTitle")
        .IsUnique();

        builder.Property(role => role.RoleTitle).HasMaxLength(30).IsRequired();
    }
}