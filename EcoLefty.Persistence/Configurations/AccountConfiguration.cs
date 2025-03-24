using EcoLefty.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoLefty.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder.Property(x => x.Id)
               .ValueGeneratedOnAdd();

        builder.HasQueryFilter(x => x.IsActive);
    }
}
