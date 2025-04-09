using AutoMapper;
using Common.Extensions;
using EcoLefty.Application;
using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.Authentication;
using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Application.Companies;
using EcoLefty.Application.Offers;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Entities;
using EcoLefty.Persistence.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace EcoLefty.IntegrationTests;

public class CompanyServiceIntegrationTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly EcoLeftyDbContext _dbContext;
    private static readonly string _dbName = "TestDB_Integration";

    #region Ctor & Service Registration
    public CompanyServiceIntegrationTests()
    {
        var services = new ServiceCollection();

        services.AddDbContext<EcoLeftyDbContext>(options =>
        options.UseInMemoryDatabase(_dbName), ServiceLifetime.Singleton);

        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .CreateLogger();
        services.AddSingleton<Serilog.ILogger>(Log.Logger);
        services.AddLogging();

        var jwtSettings = new JwtSettings
        {
            Issuer = "EcoLeftyIssuer",
            Audience = "EcoLeftyAudience",
            Secret = "SasdfasdfasdfasdfasdfasdfsduperSecretKey12345",
            ExpirationInMinutes = 60
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["JwtSettings:Issuer"] = jwtSettings.Issuer,
                ["JwtSettings:Audience"] = jwtSettings.Audience,
                ["JwtSettings:Secret"] = jwtSettings.Secret,
                ["JwtSettings:ExpirationInMinutes"] = jwtSettings.ExpirationInMinutes.ToString()
            }!)
            .Build();

        services.AddSingleton<IConfiguration>(config);

        services.ConfigureJwtSettings(config);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };
        });

        services.AddScoped<IOfferService, OfferService>();

        services.ConfigureLoggerService();
        services.ConfigureHttpLogging();
        services.ConfigureServices();
        services.ConfigureIdentity();
        services.AddAutoMapper(typeof(MappingProfile));

        var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        IMapper mapper = mappingConfig.CreateMapper();
        services.AddSingleton(mapper);

        services.AddScoped<CompanyService>();

        _serviceProvider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
        services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = httpContext });

        _dbContext = _serviceProvider.GetRequiredService<EcoLeftyDbContext>();

        ClearDatabase();
        SeedDatabase().GetAwaiter().GetResult();
    }
    #endregion

    private void ClearDatabase()
    {
        _dbContext.Companies.RemoveRange(_dbContext.Companies);
        _dbContext.Users.RemoveRange(_dbContext.Users);
        _dbContext.SaveChanges();
    }

    private async Task SeedDatabase()
    {
        var authService = _serviceProvider.GetRequiredService<IAuthenticationService>();
        var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        if (!await roleManager.RoleExistsAsync(AccountRole.Company.ToString()))
        {
            var role = new IdentityRole(AccountRole.Company.ToString());
            await roleManager.CreateAsync(role);
        }

        var registerDto1 = new RegisterAccountRequestDto
        {
            Email = "company1@eco.com",
            PhoneNumber = "1234567890",
            Password = "Test123!"
        };

        var registerDto2 = new RegisterAccountRequestDto
        {
            Email = "company2@eco.com",
            PhoneNumber = "0987654321",
            Password = "Test123!"
        };

        var token1 = await authService.RegisterAccountAsync(registerDto1, AccountRole.Company);
        var token2 = await authService.RegisterAccountAsync(registerDto2, AccountRole.Company);

        var company1Id = await authService.GetAccountIdFromJwtTokenAsync(token1.AccessToken);
        var company2Id = await authService.GetAccountIdFromJwtTokenAsync(token2.AccessToken);

        _dbContext.Companies.AddRange(
            new Company
            {
                Id = company1Id,
                Name = "Test Company 1",
                Country = "Georgia",
                City = "Tbilisi",
                Address = "test1",
                LogoUrl = "company1.png",
                Balance = 5000m,
                IsApproved = true,
                Products = new List<Product>()
            },
            new Company
            {
                Id = company2Id,
                Name = "Test Company 2",
                Country = "Georgia",
                City = "Kutaisi",
                Address = "test2",
                LogoUrl = "company2.png",
                Balance = 6500m,
                IsApproved = true,
                Products = new List<Product>()
            }
        );

        await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCompanies()
    {
        var companyService = _serviceProvider.GetRequiredService<CompanyService>();
        var companies = await companyService.GetAllAsync(CancellationToken.None);

        Assert.NotNull(companies);
        Assert.Equal(2, companies.Count());
    }

    public void Dispose()
    {
        ClearDatabase();
        _dbContext?.Dispose();
        _serviceProvider?.Dispose();
    }
}