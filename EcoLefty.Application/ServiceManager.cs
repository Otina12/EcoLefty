﻿using AutoMapper;
using EcoLefty.Application.Accounts;
using EcoLefty.Application.ApplicationUsers;
using EcoLefty.Application.Authentication;
using EcoLefty.Application.Categories;
using EcoLefty.Application.Common.Images;
using EcoLefty.Application.Companies;
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
    private readonly IImageService _imageService;

    public ServiceManager(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IMapper mapper)
    {
        _imageService = serviceProvider.GetRequiredService<IImageService>();

        var authService = serviceProvider.GetRequiredService<IAuthenticationService>();
        var transactionWrapper = serviceProvider.GetRequiredService<ITransactionWrapper>();

        _accountService = new Lazy<IAccountService>(() => new AccountService(unitOfWork));
        _applicationUserService = new Lazy<IApplicationUserService>(() => new ApplicationUserService(unitOfWork, transactionWrapper, mapper, ImageService, authService));
        _categoryService = new Lazy<ICategoryService>(() => new CategoryService(unitOfWork, mapper));
        _companyService = new Lazy<ICompanyService>(() => new CompanyService(unitOfWork, transactionWrapper, OfferService, ImageService, mapper, authService));
        _offerService = new Lazy<IOfferService>(() => new OfferService(unitOfWork, mapper));
        _productService = new Lazy<IProductService>(() => new ProductService(unitOfWork, ImageService, mapper));
        _purchaseService = new Lazy<IPurchaseService>(() => new PurchaseService(unitOfWork, mapper));
    }

    public IAccountService AccountService => _accountService.Value;
    public IApplicationUserService ApplicationUserService => _applicationUserService.Value;
    public ICategoryService CategoryService => _categoryService.Value;
    public ICompanyService CompanyService => _companyService.Value;
    public IOfferService OfferService => _offerService.Value;
    public IProductService ProductService => _productService.Value;
    public IPurchaseService PurchaseService => _purchaseService.Value;
    public IImageService ImageService => _imageService;
}