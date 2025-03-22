using EcoLefty.Domain.Entities;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoLefty.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("ApplicationUsers", Schemas.EcoLefty);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.LastName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Bio)
               .HasMaxLength(1000);

        builder.Property(x => x.ProfilePictureUrl);

        builder.Property(x => x.Balance)
               .HasPrecision(18, 2);

        builder.HasOne(x => x.IdentityUser)
               .WithOne()
               .HasForeignKey<ApplicationUser>(x => x.AuthUserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Companies)
               .WithOne(x => x.Creator)
               .HasForeignKey(x => x.CreatorId);

        builder.HasMany(x => x.FollowedCategories)
               .WithMany(x => x.FollowingUsers)
               .UsingEntity(j => j.ToTable("UserFollowedCategories"));
    }
}