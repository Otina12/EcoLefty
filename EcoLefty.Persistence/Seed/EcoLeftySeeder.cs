using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Entities;
using EcoLefty.Domain.Entities.Identity;
using EcoLefty.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EcoLefty.Persistence.Seed;

public static class EcoLeftySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EcoLeftyDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Account>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        var accountIds = await SeedIdentityAccountsAsync(userManager);
        await SeedDomainEntitiesAsync(context, accountIds);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "User", "Company" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private static async Task<Dictionary<string, string>> SeedIdentityAccountsAsync(UserManager<Account> userManager)
    {
        var accounts = new List<(string Email, string Password, AccountRole Type, string Role)>
        {
            ("admin@gmail.com", "Pass123!", AccountRole.User, "Admin"),
            ("user@gmail.com", "Pass123!", AccountRole.User, "User"),
            ("company@gmail.com", "Pass123!", AccountRole.Company, "Company")
        };

        var createdAccounts = new Dictionary<string, string>();

        foreach (var (email, password, type, role) in accounts)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing == null)
            {
                var account = new Account
                {
                    Email = email,
                    UserName = email,
                    NormalizedEmail = email.ToUpper(),
                    NormalizedUserName = email.ToUpper(),
                    EmailConfirmed = true,
                    IsActive = true,
                    AccountType = type
                };

                var result = await userManager.CreateAsync(account, password);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user '{email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                await userManager.AddToRoleAsync(account, role);
                createdAccounts[email] = account.Id;
            }
            else
            {
                createdAccounts[email] = existing.Id;
            }
        }

        return createdAccounts;
    }

    private static async Task SeedDomainEntitiesAsync(EcoLeftyDbContext context, Dictionary<string, string> accountIds)
    {
        var adminId = accountIds["admin@gmail.com"];
        var userId = accountIds["user@gmail.com"];
        var companyId = accountIds["company@gmail.com"];

        if (!context.Categories.Any())
        {
            context.Categories.AddRange(new[]
            {
                new Category { Name = "Fruits", CreatedAtUtc = DateTime.UtcNow },
                new Category { Name = "Vegetables", CreatedAtUtc = DateTime.UtcNow },
                new Category { Name = "Grains", CreatedAtUtc = DateTime.UtcNow },
                new Category { Name = "Dairy", CreatedAtUtc = DateTime.UtcNow },
                new Category { Name = "Bakery", CreatedAtUtc = DateTime.UtcNow }
            });
            await context.SaveChangesAsync();
        }

        if (!context.Companies.Any())
        {
            context.Companies.Add(new Company
            {
                Id = companyId,
                Name = "Green Foods Co.",
                City = "Copenhagen",
                Country = "Denmark",
                Address = "Organic Way 22",
                LogoUrl = "",
                Balance = 3000,
                IsApproved = true,
                CreatedAtUtc = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        if (!context.ApplicationUsers.Any())
        {
            context.ApplicationUsers.Add(new ApplicationUser
            {
                Id = userId,
                FirstName = "Eva",
                LastName = "Harvest",
                Bio = "Organic food lover",
                BirthDate = new DateTime(1992, 3, 15),
                ProfilePictureUrl = "",
                Balance = 75,
                CreatedAtUtc = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        if (!context.Products.Any())
        {
            var companyEntityId = context.Companies.First(c => c.Name == "Green Foods Co.").Id;
            context.Products.AddRange(new[]
            {
                new Product
                {
                    Name = "Organic Apple",
                    Description = "Fresh red apples from organic farms",
                    DefaultPrice = 0.99m,
                    ImageUrl = "",
                    CompanyId = companyEntityId,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Whole Grain Bread",
                    Description = "Healthy bakery product made from whole grains",
                    DefaultPrice = 2.49m,
                    ImageUrl = "",
                    CompanyId = companyEntityId,
                    CreatedAtUtc = DateTime.UtcNow
                }
            });
            await context.SaveChangesAsync();
        }

        var appleId = context.Products.First(p => p.Name == "Organic Apple").Id;
        var breadId = context.Products.First(p => p.Name == "Whole Grain Bread").Id;

        if (!context.Offers.Any())
        {
            context.Offers.AddRange(new[]
            {
                new Offer
                {
                    Title = "Apple Bundle",
                    Description = "Buy 5 apples, get 1 free",
                    UnitPrice = 4.49m,
                    TotalQuantity = 200,
                    OfferStatus = OfferStatus.Active,
                    StartDateUtc = DateTime.UtcNow,
                    ExpiryDateUtc = DateTime.UtcNow.AddDays(30),
                    ProductId = appleId,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Offer
                {
                    Title = "Bread Deal",
                    Description = "2 for 1 special on bread",
                    UnitPrice = 4.00m,
                    TotalQuantity = 100,
                    OfferStatus = OfferStatus.Active,
                    StartDateUtc = DateTime.UtcNow,
                    ExpiryDateUtc = DateTime.UtcNow.AddDays(30),
                    ProductId = breadId,
                    CreatedAtUtc = DateTime.UtcNow
                }
            });
            await context.SaveChangesAsync();
        }
    }
}
