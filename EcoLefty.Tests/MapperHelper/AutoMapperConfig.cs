using AutoMapper;
using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.Categories.DTOs;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Application.Offers.DTOs;
using EcoLefty.Application.Products.DTOs;
using EcoLefty.Application.Purchases.DTOs;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Tests.MapperHelper;

public static class AutoMapperConfig
{
    private static IMapper _mapper;

    public static IMapper Initialize()
    {
        if (_mapper == null)
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ApplicationMappingProfile>();
            });

            _mapper = configuration.CreateMapper();
        }

        return _mapper;
    }
}

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        // Account & User
        CreateMap<CreateApplicationUserRequestDto, RegisterAccountRequestDto>();

        // Account & Company
        CreateMap<CreateCompanyRequestDto, RegisterAccountRequestDto>();

        // User Mappings
        ConfigureUserMappings();

        // Company Mappings
        ConfigureCompanyMappings();

        // Category Mappings
        ConfigureCategoryMappings();

        // Product Mappings
        ConfigureProductMappings();

        // Offer Mappings
        ConfigureOfferMappings();

        // Purchase Mappings
        ConfigurePurchaseMappings();
    }

    private void ConfigureUserMappings()
    {
        CreateMap<ApplicationUser, ApplicationUserResponseDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email));

        CreateMap<ApplicationUser, ApplicationUserDetailsResponseDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber));

        CreateMap<CreateApplicationUserRequestDto, ApplicationUser>()
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => new DateTime(src.BirthYear, src.BirthMonth, src.BirthDay)))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Balance, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore())
            .ForMember(dest => dest.FollowedCategories, opt => opt.Ignore())
            .ForMember(dest => dest.Purchases, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());

        CreateMap<UpdateApplicationUserRequestDto, ApplicationUser>()
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Balance, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore())
            .ForMember(dest => dest.BirthDate, opt => opt.Ignore())
            .ForMember(dest => dest.FollowedCategories, opt => opt.Ignore())
            .ForMember(dest => dest.Purchases, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());

        CreateMap<ApplicationUserDetailsResponseDto, UpdateApplicationUserRequestDto>()
            .ForMember(dest => dest.ProfilePictureFile, opt => opt.Ignore());
    }

    private void ConfigureCompanyMappings()
    {
        CreateMap<Company, CompanyResponseDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email));

        CreateMap<Company, CompanyDetailsResponseDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber))
            .ForMember(dest => dest.Offers, opt => opt.Ignore());

        CreateMap<CreateCompanyRequestDto, Company>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.LogoUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Balance, opt => opt.Ignore())
            .ForMember(dest => dest.IsApproved, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());

        CreateMap<UpdateCompanyRequestDto, Company>()
            .ForMember(dest => dest.LogoUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Balance, opt => opt.Ignore())
            .ForMember(dest => dest.IsApproved, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());

        CreateMap<CompanyDetailsResponseDto, UpdateCompanyRequestDto>()
            .ForMember(dest => dest.LogoFile, opt => opt.Ignore());
    }

    private void ConfigureCategoryMappings()
    {
        CreateMap<Category, CategoryResponseDto>();

        CreateMap<CreateCategoryRequestDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.FollowingUsers, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());

        CreateMap<UpdateCategoryRequestDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.FollowingUsers, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());
    }

    private void ConfigureProductMappings()
    {
        CreateMap<Product, ProductResponseDto>();

        CreateMap<Product, ProductDetailsResponseDto>();

        CreateMap<CreateProductRequestDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
            .ForMember(dest => dest.Company, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Offers, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());

        CreateMap<UpdateProductRequestDto, Product>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
            .ForMember(dest => dest.Company, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Offers, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());

        CreateMap<ProductDetailsResponseDto, UpdateProductRequestDto>()
            .ForMember(dest => dest.ImageFile, opt => opt.Ignore());
    }

    private void ConfigureOfferMappings()
    {
        CreateMap<Offer, OfferResponseDto>();

        CreateMap<Offer, OfferDetailsResponseDto>()
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Product.Company))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Product.Categories));

        CreateMap<CreateOfferRequestDto, Offer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.QuantityAvailable, opt => opt.Ignore())
            .ForMember(dest => dest.OfferStatus, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.Purchases, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());

        CreateMap<UpdateOfferRequestDto, Offer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.QuantityAvailable, opt => opt.Ignore())
            .ForMember(dest => dest.OfferStatus, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.Purchases, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());

        CreateMap<OfferDetailsResponseDto, UpdateOfferRequestDto>();
    }

    private void ConfigurePurchaseMappings()
    {
        CreateMap<Purchase, PurchaseDetailsResponseDto>();

        CreateMap<CreatePurchaseRequestDto, Purchase>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PurchaseStatus, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.Offer, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());
    }
}