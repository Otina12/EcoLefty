﻿using EcoLefty.Domain.Entities;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoLefty.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(Tables.Product, Schemas.EcoLefty);

        builder.HasQueryFilter(x => x.DeletedAtUtc == null);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Description)
               .HasMaxLength(1000);

        builder.Property(x => x.DefaultPrice)
               .HasPrecision(18, 2);

        builder.HasOne(x => x.Company)
               .WithMany(x => x.Products)
               .HasForeignKey(x => x.CompanyId);

        builder.HasMany(x => x.Offers)
               .WithOne(x => x.Product)
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Categories)
               .WithMany(x => x.Products)
               .UsingEntity(j => j.ToTable("ProductCategories"));
    }
}