using AutoMapper;
using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.Categories.DTOs;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Application.Products.DTOs;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Account & User
        CreateMap<ApplicationUser, ApplicationUserResponseDto>()
            .ForMember(dest => dest.Email, src => src.MapFrom(src => src.Account.Email));

        CreateMap<ApplicationUser, ApplicationUserDetailsResponseDto>()
            .ForMember(dest => dest.Email, src => src.MapFrom(src => src.Account.Email))
            .ForMember(dest => dest.PhoneNumber, src => src.MapFrom(src => src.Account.PhoneNumber));

        CreateMap<CreateApplicationUserRequestDto, RegisterAccountRequestDto>();

        // Account & Company
        CreateMap<Company, CompanyResponseDto>()
            .ForMember(dest => dest.Email, src => src.MapFrom(src => src.Account.Email));

        CreateMap<Company, CompanyDetailsResponseDto>()
            .ForMember(dest => dest.Email, src => src.MapFrom(src => src.Account.Email))
            .ForMember(dest => dest.PhoneNumber, src => src.MapFrom(src => src.Account.PhoneNumber));

        CreateMap<CreateCompanyRequestDto, RegisterAccountRequestDto>();

        // Category
        CreateMap<Category, CategoryResponseDto>();
        CreateMap<CreateCategoryRequestDto, Category>();
        CreateMap<UpdateCategoryRequestDto, Category>();

        // Product
        CreateMap<Product, ProductResponseDto>();
        CreateMap<Product, ProductDetailsResponseDto>();
        CreateMap<CreateProductRequestDto, Product>();
        CreateMap<UpdateProductRequestDto, Product>();


    }
}
