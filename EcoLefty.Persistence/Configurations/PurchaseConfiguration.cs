using EcoLefty.Domain.Entities;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoLefty.Persistence.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable(Tables.Purchase, Schemas.EcoLefty);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TotalPrice)
               .HasPrecision(18, 2);

        builder.HasOne(p => p.Offer)
               .WithMany(o => o.Purchases)
               .HasForeignKey(p => p.OfferId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Customer)
               .WithMany(c => c.Purchases)
               .HasForeignKey(p => p.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
