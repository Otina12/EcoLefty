using AutoMapper;
using EcoLefty.Application.Accounts;
using EcoLefty.Application.ApplicationUsers;
using EcoLefty.Application.Authentication;
using EcoLefty.Application.Categories;
using EcoLefty.Application.Companies;
using EcoLefty.Application.Contracts;
using EcoLefty.Application.Offers;
using EcoLefty.Application.Products;
using EcoLefty.Application.Purchases;
using EcoLefty.Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace EcoLefty.Application;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAccountService> _accountService;
    private readonly Lazy<IApplicationUserService> _applicationUserService;
    private readonly Lazy<ICategoryService> _categoryService;
    private readonly Lazy<ICompanyService> _companyService;
    private readonly Lazy<IOfferService> _offerService;
    private readonly Lazy<IProductService> _productService;
    private readonly Lazy<IPurchaseService> _purchaseService;

    public ServiceManager(IUnitOfWork unitOfWork,
        IMapper mapper,
        IServiceProvider serviceProvider)
    {
        var authService = serviceProvider.GetRequiredService<IAuthenticationService>();

        _accountService = new Lazy<IAccountService>(() => new AccountService(unitOfWork));
        _applicationUserService = new Lazy<IApplicationUserService>(() => new ApplicationUserService(unitOfWork, mapper, authService));
        _categoryService = new Lazy<ICategoryService>(() => new CategoryService(unitOfWork, mapper));
        _companyService = new Lazy<ICompanyService>(() => new CompanyService(unitOfWork, mapper, authService));
        _offerService = new Lazy<IOfferService>(() => new OfferService(unitOfWork, mapper, PurchaseService));
        _productService = new Lazy<IProductService>(() => new ProductService(unitOfWork, mapper));
        _purchaseService = new Lazy<IPurchaseService>(() => new PurchaseService(unitOfWork, mapper));
    }

    public IAccountService AccountService => _accountService.Value;
    public IApplicationUserService ApplicationUserService => _applicationUserService.Value;
    public ICategoryService CategoryService => _categoryService.Value;
    public ICompanyService CompanyService => _companyService.Value;
    public IOfferService OfferService => _offerService.Value;
    public IProductService ProductService => _productService.Value;
    public IPurchaseService PurchaseService => _purchaseService.Value;
}