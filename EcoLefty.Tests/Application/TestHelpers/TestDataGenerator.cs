using Bogus;
using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.Categories.DTOs;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Application.Offers.DTOs;
using EcoLefty.Application.Products.DTOs;
using EcoLefty.Application.Purchases.DTOs;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Entities;
using EcoLefty.Domain.Entities.Identity;

namespace EcoLefty.Tests.Application.TestHelpers;

public static class TestDataGenerator
{
    private static readonly Faker Faker = new Faker("en");

    public static Account GenerateAccount(string id = null, string email = null, bool isCompany = false)
    {
        return new Account
        {
            Id = id ?? Guid.NewGuid().ToString(),
            UserName = email ?? Faker.Internet.Email(),
            Email = email ?? Faker.Internet.Email(),
            EmailConfirmed = Faker.Random.Bool(),
            PhoneNumber = Faker.Phone.PhoneNumber(),
            PhoneNumberConfirmed = Faker.Random.Bool(),
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0,
            AccountType = isCompany ? AccountRole.Company : AccountRole.User,
        };
    }

    public static ApplicationUser GenerateApplicationUser(string id = null, Account account = null)
    {
        id = id ?? Guid.NewGuid().ToString();
        var createdAt = DateTime.UtcNow.AddDays(-Faker.Random.Int(1, 365));

        return new ApplicationUser
        {
            Id = id,
            FirstName = Faker.Name.FirstName(),
            LastName = Faker.Name.LastName(),
            Bio = Faker.Lorem.Paragraph(),
            BirthDate = Faker.Date.Past(30, DateTime.UtcNow.AddYears(-18)),
            ProfilePictureUrl = Faker.Internet.Avatar(),
            Balance = Faker.Random.Decimal(100, 5000),
            Account = account ?? GenerateAccount(id, isCompany: false),
            FollowedCategories = new List<Category>(),
            Purchases = new List<Purchase>(),
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = Faker.Date.Between(createdAt, DateTime.UtcNow),
            DeletedAtUtc = null
        };
    }

    public static Company GenerateCompany(string id = null, Account account = null)
    {
        id = id ?? Guid.NewGuid().ToString();
        var createdAt = DateTime.UtcNow.AddDays(-Faker.Random.Int(1, 365));

        return new Company
        {
            Id = id,
            Name = Faker.Company.CompanyName(),
            Country = Faker.Address.Country(),
            City = Faker.Address.City(),
            Address = Faker.Address.FullAddress(),
            LogoUrl = $"https://logo.clearbit.com/{Faker.Internet.DomainName()}",
            Balance = Faker.Random.Decimal(1000, 100000),
            IsApproved = Faker.Random.Bool(0.9f), // 90% of companies are approved
            Account = account ?? GenerateAccount(id, isCompany: true),
            Products = new List<Product>(),
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = Faker.Date.Between(createdAt, DateTime.UtcNow),
            DeletedAtUtc = null
        };
    }

    public static Category GenerateCategory(int id = 0)
    {
        var createdAt = DateTime.UtcNow.AddDays(-Faker.Random.Int(1, 365));
        var categoryNames = new[] { "Organic", "Eco-friendly", "Sustainable", "Recycled", "Biodegradable",
            "Fair Trade", "Vegan", "Zero Waste", "Energy Efficient", "Cruelty-Free", "Local", "Handmade" };

        return new Category
        {
            Id = id == 0 ? Faker.Random.Int(1, 100) : id,
            Name = Faker.PickRandom(categoryNames),
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = Faker.Date.Between(createdAt, DateTime.UtcNow),
            DeletedAtUtc = null
        };
    }

    public static Product GenerateProduct(int id = 0, string companyId = null, Company company = null)
    {
        var createdAt = DateTime.UtcNow.AddDays(-Faker.Random.Int(1, 365));
        var environmentallyFriendlyAdjectives = new[] { "Sustainable", "Eco-friendly", "Green", "Organic", "Recycled" };
        var productTypes = new[] { "Bag", "Bottle", "Clothing", "Cleaning Product", "Food", "Cosmetics", "Home Goods" };

        var productName = $"{Faker.PickRandom(environmentallyFriendlyAdjectives)} {Faker.PickRandom(productTypes)}";

        companyId = companyId ?? Guid.NewGuid().ToString();
        company = company ?? GenerateCompany(companyId);

        return new Product
        {
            Id = id == 0 ? Faker.Random.Int(1, 1000) : id,
            Name = productName,
            Description = Faker.Commerce.ProductDescription(),
            DefaultPrice = Math.Round(Faker.Random.Decimal(10, 1000), 2),
            ImageUrl = $"https://picsum.photos/seed/{Guid.NewGuid()}/500/500",
            CompanyId = company.Id,
            Company = company,
            Categories = new List<Category>(),
            Offers = new List<Offer>(),
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = Faker.Date.Between(createdAt, DateTime.UtcNow),
            DeletedAtUtc = null
        };
    }

    public static Offer GenerateOffer(int id = 0, int productId = 0, Product product = null)
    {
        var createdAt = DateTime.UtcNow.AddDays(-Faker.Random.Int(1, 30));

        if (product == null)
        {
            product = GenerateProduct(productId == 0 ? Faker.Random.Int(1, 1000) : productId);
        }

        var discountPercentage = Faker.Random.Decimal(0.70m, 0.95m);
        var unitPrice = Math.Round(product.DefaultPrice * discountPercentage, 2);

        var startDate = DateTime.UtcNow.AddDays(-Faker.Random.Int(0, 15));
        var expiryDate = startDate.AddDays(Faker.Random.Int(15, 60));

        var offerStatus = OfferStatus.Active;

        if (expiryDate < DateTime.UtcNow)
        {
            offerStatus = OfferStatus.Archived;
        }

        var totalQuantity = Faker.Random.Int(50, 500);
        var soldQuantity = offerStatus == OfferStatus.Active ? Faker.Random.Int(0, totalQuantity / 2) : 0;

        return new Offer
        {
            Id = id == 0 ? Faker.Random.Int(1, 1000) : id,
            Title = $"Special Offer: {product.Name}",
            Description = Faker.Lorem.Paragraph(),
            UnitPrice = unitPrice,
            TotalQuantity = totalQuantity,
            QuantityAvailable = totalQuantity - soldQuantity,
            OfferStatus = offerStatus,
            StartDateUtc = startDate,
            ExpiryDateUtc = expiryDate,
            ProductId = product.Id,
            Product = product,
            Purchases = new List<Purchase>(),
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = Faker.Date.Between(createdAt, DateTime.UtcNow),
            DeletedAtUtc = null
        };
    }

    public static Purchase GeneratePurchase(int id = 0, string customerId = null, int offerId = 0)
    {
        var createdAt = DateTime.UtcNow.AddDays(-Faker.Random.Int(0, 30));

        var customer = customerId == null ? GenerateApplicationUser() : GenerateApplicationUser(customerId);
        var offer = offerId == 0 ? GenerateOffer() : GenerateOffer(offerId);

        var quantity = Faker.Random.Int(1, Math.Min(5, offer.QuantityAvailable));
        var totalPrice = Math.Round(quantity * offer.UnitPrice, 2);
        var purchaseStatus = Faker.PickRandom<PurchaseStatus>();

        var purchaseDate = purchaseStatus switch
        {
            PurchaseStatus.Cancelled => Faker.Date.Between(createdAt, DateTime.UtcNow.AddDays(-2)),
            PurchaseStatus.Delivered => Faker.Date.Between(createdAt, DateTime.UtcNow.AddDays(-1)),
            PurchaseStatus.Active => Faker.Date.Between(createdAt, DateTime.UtcNow.AddDays(-3)),
            _ => Faker.Date.Recent(7)
        };

        return new Purchase
        {
            Id = id == 0 ? Faker.Random.Int(1, 1000) : id,
            PurchaseStatus = purchaseStatus,
            Quantity = quantity,
            TotalPrice = totalPrice,
            PurchaseDateUtc = purchaseDate,
            OfferId = offer.Id,
            Offer = offer,
            CustomerId = customer.Id,
            Customer = customer,
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = Faker.Date.Between(createdAt, DateTime.UtcNow),
            DeletedAtUtc = null
        };
    }

    public static List<Purchase> GeneratePurchases(int count = 5)
    {
        var result = new List<Purchase>();
        for (int i = 0; i < count; i++)
        {
            result.Add(GeneratePurchase(i + 1));
        }
        return result;
    }

    public static List<Purchase> GeneratePurchasesForOffer(int offerId, int count = 3)
    {
        var offer = GenerateOffer(offerId);
        var result = new List<Purchase>();

        for (int i = 0; i < count; i++)
        {
            var purchase = GeneratePurchase(i + 1, null, offerId);
            purchase.Offer = offer;
            result.Add(purchase);
        }

        int totalPurchased = result.Sum(p => p.Quantity);
        offer.QuantityAvailable = offer.TotalQuantity - totalPurchased;

        return result;
    }

    public static List<Purchase> GeneratePurchasesForCustomer(string customerId, int count = 3)
    {
        var customer = GenerateApplicationUser(customerId);
        var result = new List<Purchase>();

        for (int i = 0; i < count; i++)
        {
            var purchase = GeneratePurchase(i + 1, customerId);
            purchase.Customer = customer;
            result.Add(purchase);
        }

        return result;
    }

    public static (List<ApplicationUser> Customers, List<Company> Companies, List<Product> Products, List<Offer> Offers, List<Purchase> Purchases)
        GenerateCompleteScenario(int customerCount = 3, int companyCount = 2, int productsPerCompany = 2, int offersPerProduct = 1, int purchasesPerCustomer = 3)
    {
        var customers = new List<ApplicationUser>();
        var companies = new List<Company>();
        var products = new List<Product>();
        var offers = new List<Offer>();
        var purchases = new List<Purchase>();

        for (int i = 0; i < customerCount; i++)
        {
            customers.Add(GenerateApplicationUser());
        }

        for (int i = 0; i < companyCount; i++)
        {
            var company = GenerateCompany();
            companies.Add(company);

            for (int j = 0; j < productsPerCompany; j++)
            {
                var product = GenerateProduct(0, company.Id, company);
                products.Add(product);
                company.Products.Add(product);

                var categoryCount = Faker.Random.Int(1, 3);
                for (int k = 0; k < categoryCount; k++)
                {
                    var category = GenerateCategory();
                    product.Categories.Add(category);
                }

                for (int k = 0; k < offersPerProduct; k++)
                {
                    var offer = GenerateOffer(0, product.Id, product);
                    offers.Add(offer);
                    product.Offers.Add(offer);
                }
            }
        }

        int purchaseId = 1;
        foreach (var customer in customers)
        {
            for (int i = 0; i < purchasesPerCustomer; i++)
            {
                var offer = Faker.PickRandom(offers);

                var purchase = new Purchase
                {
                    Id = purchaseId++,
                    PurchaseStatus = Faker.PickRandom<PurchaseStatus>(),
                    Quantity = Faker.Random.Int(1, 3),
                    OfferId = offer.Id,
                    Offer = offer,
                    CustomerId = customer.Id,
                    Customer = customer,
                    PurchaseDateUtc = Faker.Date.Recent(30),
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-Faker.Random.Int(1, 30)),
                    UpdatedAtUtc = DateTime.UtcNow.AddDays(-Faker.Random.Int(0, 5)),
                    DeletedAtUtc = null
                };

                purchase.TotalPrice = Math.Round(purchase.Quantity * offer.UnitPrice, 2);

                purchases.Add(purchase);
                customer.Purchases.Add(purchase);
                offer.Purchases.Add(purchase);

                offer.QuantityAvailable -= purchase.Quantity;
            }
        }

        return (customers, companies, products, offers, purchases);
    }

    public static CreatePurchaseRequestDto GenerateCreatePurchaseRequestDto(int offerId = 0, int quantity = 0)
    {
        return new CreatePurchaseRequestDto
        {
            OfferId = offerId == 0 ? Faker.Random.Int(1, 100) : offerId,
            PurchaseDateUtc = DateTime.UtcNow.AddDays(-Faker.Random.Int(1, 30)),
            Quantity = quantity == 0 ? Faker.Random.Int(1, 5) : quantity
        };
    }

    public static PurchaseDetailsResponseDto GeneratePurchaseDetailsResponseDto(int id = 0)
    {
        var purchase = GeneratePurchase(id);

        // Create CategoryResponseDto objects
        var categories = purchase.Offer.Product.Categories.Select(c => new CategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();

        // Create CompanyResponseDto
        var companyDto = new CompanyResponseDto
        {
            Id = purchase.Offer.Product.Company.Id,
            Name = purchase.Offer.Product.Company.Name,
            Email = purchase.Offer.Product.Company.Account.Email ?? "",
            Country = purchase.Offer.Product.Company.Country,
            City = purchase.Offer.Product.Company.City,
            Address = purchase.Offer.Product.Company.Address,
            LogoUrl = purchase.Offer.Product.Company.LogoUrl ?? "",
            Balance = purchase.Offer.Product.Company.Balance,
            IsApproved = purchase.Offer.Product.Company.IsApproved
        };

        var productDto = new ProductResponseDto
        {
            Id = purchase.Offer.Product.Id,
            Name = purchase.Offer.Product.Name,
            Description = purchase.Offer.Product.Description,
            DefaultPrice = purchase.Offer.Product.DefaultPrice,
            ImageUrl = purchase.Offer.Product.ImageUrl,
            Company = companyDto
        };

        var offerDto = new OfferDetailsResponseDto
        {
            Id = purchase.Offer.Id,
            Title = purchase.Offer.Title,
            Description = purchase.Offer.Description,
            UnitPrice = purchase.Offer.UnitPrice,
            TotalQuantity = purchase.Offer.TotalQuantity,
            QuantityAvailable = purchase.Offer.QuantityAvailable,
            OfferStatus = purchase.Offer.OfferStatus,
            StartDateUtc = purchase.Offer.StartDateUtc,
            ExpiryDateUtc = purchase.Offer.ExpiryDateUtc,
            Company = companyDto,
            Product = productDto,
            Categories = categories
        };

        var customerDto = new ApplicationUserResponseDto
        {
            Id = purchase.Customer.Id,
            FirstName = purchase.Customer.FirstName,
            LastName = purchase.Customer.LastName,
            Email = purchase.Customer.Account.Email ?? "",
            Bio = purchase.Customer.Bio,
            ProfilePictureUrl = purchase.Customer.ProfilePictureUrl ?? "",
            Balance = purchase.Customer.Balance
        };

        return new PurchaseDetailsResponseDto
        {
            Id = purchase.Id,
            PurchaseStatus = purchase.PurchaseStatus,
            Quantity = purchase.Quantity,
            TotalPrice = purchase.TotalPrice,
            PurchaseDateUtc = purchase.PurchaseDateUtc,
            Offer = offerDto,
            Customer = customerDto
        };
    }

    public static List<PurchaseDetailsResponseDto> GeneratePurchaseDetailsResponseDtos(int count = 5)
    {
        var result = new List<PurchaseDetailsResponseDto>();
        for (int i = 0; i < count; i++)
        {
            result.Add(GeneratePurchaseDetailsResponseDto(i + 1));
        }
        return result;
    }
}