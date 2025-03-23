using EcoLefty.Domain.Entities;
using EcoLefty.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcoLefty.Persistence.Context;

public class EcoLeftyDbContext : IdentityDbContext<Account, IdentityRole, string>
{
    public EcoLeftyDbContext(DbContextOptions<EcoLeftyDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Account>().ToTable("Accounts", Schemas.Auth);
        builder.Entity<IdentityRole>().ToTable("Roles", Schemas.Auth);
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", Schemas.Auth);
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", Schemas.Auth);
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", Schemas.Auth);
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", Schemas.Auth);
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", Schemas.Auth);

        builder.ApplyConfigurationsFromAssembly(typeof(EcoLeftyDbContext).Assembly);
    }
}