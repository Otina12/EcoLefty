using EcoLefty.Domain.Entities;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoLefty.Persistence.Configurations;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offers", Schemas.EcoLefty);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Description)
               .HasMaxLength(1000);

        builder.Property(x => x.UnitPrice)
               .HasPrecision(18, 2);

        builder.HasOne(x => x.Company)
               .WithMany(x => x.Offers)
               .HasForeignKey(x => x.CompanyId);

        builder.HasOne(x => x.Product)
               .WithMany(x => x.Offers)
               .HasForeignKey(x => x.ProductId);
    }
}