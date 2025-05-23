﻿using EcoLefty.Domain.Entities;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoLefty.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable(Tables.Company, Schemas.EcoLefty);

        builder.HasQueryFilter(x => x.DeletedAtUtc == null);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Country)
               .HasMaxLength(100);

        builder.Property(x => x.City)
               .HasMaxLength(100);

        builder.Property(x => x.Address)
               .HasMaxLength(300);

        builder.Property(x => x.Balance)
               .HasPrecision(18, 2);

        builder.HasOne(x => x.Account)
               .WithOne()
               .HasForeignKey<Company>(x => x.Id)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Products)
               .WithOne(x => x.Company)
               .HasForeignKey(x => x.CompanyId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}