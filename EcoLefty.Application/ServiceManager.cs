using AutoMapper;
using EcoLefty.Application.Accounts;
using EcoLefty.Application.ApplicationUsers;
using EcoLefty.Application.Categories;
using EcoLefty.Application.Companies;
using EcoLefty.Application.Offers;
using EcoLefty.Application.Products;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
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

    public ServiceManager(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
    {
        _accountService = new Lazy<IAccountService>(() =>
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Account>>();
            var signInManager = serviceProvider.GetRequiredService<SignInManager<Account>>();
            return new AccountService(userManager, signInManager, mapper);
        });

        _applicationUserService = new Lazy<IApplicationUserService>(() => new ApplicationUserService(unitOfWork, mapper, AuthService));
        _categoryService = new Lazy<ICategoryService>(() => new CategoryService(unitOfWork, mapper));
        _companyService = new Lazy<ICompanyService>(() => new CompanyService(unitOfWork, mapper, AuthService));
        _offerService = new Lazy<IOfferService>(() => new OfferService(unitOfWork, mapper));
        _productService = new Lazy<IProductService>(() => new ProductService(unitOfWork, mapper));

    }

    public IAccountService AuthService => _accountService.Value;
    public IApplicationUserService ApplicationUserService => _applicationUserService.Value;
    public ICategoryService CategoryService => _categoryService.Value;
    public ICompanyService CompanyService => _companyService.Value;
    public IOfferService OfferService => _offerService.Value;
    public IProductService ProductService => _productService.Value;
}