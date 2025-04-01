using EcoLefty.Application.Accounts;
using EcoLefty.Application.ApplicationUsers;
using EcoLefty.Application.Categories;
using EcoLefty.Application.Common.Images;
using EcoLefty.Application.Companies;
using EcoLefty.Application.Offers;
using EcoLefty.Application.Products;
using EcoLefty.Application.Purchases;

namespace EcoLefty.Application.Contracts;

public interface IServiceManager
{
    IAccountService AccountService { get; }
    IApplicationUserService ApplicationUserService { get; }
    ICategoryService CategoryService { get; }
    ICompanyService CompanyService { get; }
    IOfferService OfferService { get; }
    IProductService ProductService { get; }
    IPurchaseService PurchaseService { get; }
    public IImageService ImageService { get; }
}
