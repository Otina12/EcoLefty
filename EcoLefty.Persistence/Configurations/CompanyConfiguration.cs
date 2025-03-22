using EcoLefty.Domain.Entities;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoLefty.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies", Schemas.EcoLefty);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Phone)
               .HasMaxLength(30);

        builder.Property(x => x.Country)
               .HasMaxLength(100);

        builder.Property(x => x.City)
               .HasMaxLength(100);

        builder.Property(x => x.Address)
               .HasMaxLength(300);

        builder.Property(x => x.Balance)
               .HasPrecision(18, 2);

        builder.HasOne(x => x.Creator)
               .WithMany(x => x.Companies)
               .HasForeignKey(x => x.CreatorId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Products)
               .WithOne(x => x.Company)
               .HasForeignKey(x => x.CompanyId);

        builder.HasMany(x => x.Offers)
               .WithOne(x => x.Company)
               .HasForeignKey(x => x.CompanyId);
    }
}