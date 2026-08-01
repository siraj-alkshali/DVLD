using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.RefreshTokenID);

        builder.HasOne(rt => rt.User)
        .WithMany(u => u.RefreshTokens)
        .HasForeignKey(rt => rt.UserID)
        .OnDelete(DeleteBehavior.Cascade)
        .HasConstraintName("FK_RefreshTokens_Users");

        builder.Property(rt => rt.TokenHash).HasMaxLength(255).IsRequired();
        builder.Property(rt => rt.ReplacedByTokenHash).HasMaxLength(255);

        builder.HasIndex(rt => rt.TokenHash)
        .HasDatabaseName("IX_RefreshTokens_TokenHash");

        builder.HasIndex(rt => rt.UserID)
        .HasDatabaseName("IX_RefreshTokens_UserID");
    }
}