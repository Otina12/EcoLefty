using AutoMapper;
using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.Categories.DTOs;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Application.Offers.DTOs;
using EcoLefty.Application.Products.DTOs;
using EcoLefty.Application.Purchases.DTOs;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Account & User
        CreateMap<CreateApplicationUserRequestDto, RegisterAccountRequestDto>();

        // Account & Company
        CreateMap<CreateCompanyRequestDto, RegisterAccountRequestDto>();

        // User
        CreateMap<ApplicationUser, ApplicationUserResponseDto>()
            .ForMember(dest => dest.Email, src => src.MapFrom(src => src.Account.Email));

        CreateMap<ApplicationUser, ApplicationUserDetailsResponseDto>()
            .ForMember(dest => dest.Email, src => src.MapFrom(src => src.Account.Email))
            .ForMember(dest => dest.PhoneNumber, src => src.MapFrom(src => src.Account.PhoneNumber));

        CreateMap<CreateApplicationUserRequestDto, ApplicationUser>()
            .ForMember(dest => dest.BirthDate, src => src.MapFrom(src => new DateTime(src.BirthYear, src.BirthMonth, src.BirthDay)));

        CreateMap<UpdateApplicationUserRequestDto, ApplicationUser>()
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore());

        CreateMap<ApplicationUserDetailsResponseDto, UpdateApplicationUserRequestDto>();

        // Company
        CreateMap<Company, CompanyResponseDto>()
            .ForMember(dest => dest.Email, src => src.MapFrom(src => src.Account.Email));

        CreateMap<Company, CompanyDetailsResponseDto>()
            .ForMember(dest => dest.Email, src => src.MapFrom(src => src.Account.Email))
            .ForMember(dest => dest.PhoneNumber, src => src.MapFrom(src => src.Account.PhoneNumber));

        CreateMap<CreateCompanyRequestDto, Company>();
        CreateMap<UpdateCompanyRequestDto, Company>()
            .ForMember(dest => dest.LogoUrl, opt => opt.Ignore());
        CreateMap<CompanyDetailsResponseDto, UpdateCompanyRequestDto>();

        // Category
        CreateMap<Category, CategoryResponseDto>();
        CreateMap<CreateCategoryRequestDto, Category>();
        CreateMap<UpdateCategoryRequestDto, Category>();

        // Product
        CreateMap<Product, ProductResponseDto>();
        CreateMap<Product, ProductDetailsResponseDto>();
        CreateMap<CreateProductRequestDto, Product>();
        CreateMap<UpdateProductRequestDto, Product>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

        CreateMap<ProductDetailsResponseDto, UpdateProductRequestDto>();

        // Offer
        CreateMap<Offer, OfferResponseDto>();
        CreateMap<Offer, OfferDetailsResponseDto>()
            .ForMember(dest => dest.Company, src => src.MapFrom(src => src.Product.Company))
            .ForMember(dest => dest.Categories, src => src.MapFrom(src => src.Product.Categories));

        CreateMap<CreateOfferRequestDto, Offer>();
        CreateMap<UpdateOfferRequestDto, Offer>();
        CreateMap<OfferDetailsResponseDto, UpdateOfferRequestDto>();

        // Purchase
        CreateMap<Purchase, PurchaseDetailsResponseDto>();
        CreateMap<CreatePurchaseRequestDto, Purchase>();
    }
}
