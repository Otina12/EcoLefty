using EcoLefty.Domain.Entities;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoLefty.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable(Tables.AuditLog, Schemas.EcoLefty);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
               .IsRequired();

        builder.Property(x => x.EntityName)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.EntityId)
               .IsRequired();

        builder.Property(x => x.Changes)
               .IsRequired();

        builder.Property(x => x.Timestamp)
               .IsRequired();
    }
}