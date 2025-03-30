using EcoLefty.Domain.Entities.Auth;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoLefty.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable(Tables.RefreshToken, Schemas.Auth);

        builder.HasKey(p => p.Id);

        builder.Property(x => x.Token).HasMaxLength(200);

        builder.HasIndex(x => x.Token).IsUnique();

        builder.HasOne(p => p.Account)
               .WithMany()
               .HasForeignKey(p => p.AccountId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
