using EcoLefty.Application.Accounts;
using EcoLefty.Application.ApplicationUsers;
using EcoLefty.Application.Categories;
using EcoLefty.Application.Companies;
using EcoLefty.Application.Offers;
using EcoLefty.Application.Products;

namespace EcoLefty.Application;

public interface IServiceManager
{
    IAccountService AuthService { get; }
    IApplicationUserService ApplicationUserService { get; }
    ICategoryService CategoryService { get; }
    ICompanyService CompanyService { get; }
    IOfferService OfferService { get; }
    IProductService ProductService { get; }
}
