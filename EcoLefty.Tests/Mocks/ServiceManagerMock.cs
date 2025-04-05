using AutoMapper;
using EcoLefty.Application;
using EcoLefty.Application.Accounts;
using EcoLefty.Application.ApplicationUsers;
using EcoLefty.Application.Authentication;
using EcoLefty.Application.Categories;
using EcoLefty.Application.Common.Images;
using EcoLefty.Application.Companies;
using EcoLefty.Application.Offers;
using EcoLefty.Application.Products;
using EcoLefty.Application.Purchases;
using Moq;

namespace EcoLefty.Tests.Mocks;

public class ServiceManagerMock
{
    private ServiceManager? _serviceManager;
    private readonly UnitOfWorkMock _unitOfWorkMock = new UnitOfWorkMock();
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;

    private IAccountService? _accountService;
    private IApplicationUserService? _applicationUserService;
    private ICategoryService? _categoryService;
    private ICompanyService? _companyService;
    private IOfferService? _offerService;
    private IProductService? _productService;
    private IPurchaseService? _purchaseService;
    private IImageService? _imageService;

    public ServiceManagerMock()
    {
        var mapperMock = new Mock<IMapper>();
        _mapper = mapperMock.Object;

        var imageServiceMock = new Mock<IImageService>();
        _imageService = imageServiceMock.Object;
        var authServiceMock = new Mock<IAuthenticationService>();

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IImageService)))
            .Returns(_imageService);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);
        _serviceProvider = serviceProviderMock.Object;
    }

    public ServiceManager GetServiceManager()
    {
        if (_serviceManager != null)
            return _serviceManager;

        var serviceMock = new Mock<IServiceManager>();

        serviceMock.Setup(s => s.AccountService).Returns(GetAccountService());
        serviceMock.Setup(s => s.ApplicationUserService).Returns(GetApplicationUserService());
        serviceMock.Setup(s => s.CategoryService).Returns(GetCategoryService());
        serviceMock.Setup(s => s.CompanyService).Returns(GetCompanyService());
        serviceMock.Setup(s => s.OfferService).Returns(GetOfferService());
        serviceMock.Setup(s => s.ProductService).Returns(GetProductService());
        serviceMock.Setup(s => s.PurchaseService).Returns(GetPurchaseService());
        serviceMock.Setup(s => s.ImageService).Returns(GetImageService());

        _serviceManager = new ServiceManager(_unitOfWorkMock.UnitOfWorkInstance, _mapper, _serviceProvider);
        return _serviceManager;
    }

    #region Service Mocks

    public IAccountService GetAccountService()
    {
        if (_accountService != null)
            return _accountService;

        _accountService = new AccountService(_unitOfWorkMock.UnitOfWorkInstance);
        return _accountService;
    }

    public IApplicationUserService GetApplicationUserService()
    {
        if (_applicationUserService != null)
            return _applicationUserService;

        _applicationUserService = new ApplicationUserService(
            _unitOfWorkMock.UnitOfWorkInstance,
            _mapper,
            GetImageService(),
            GetAuthenticationService());
        return _applicationUserService;
    }

    public ICategoryService GetCategoryService()
    {
        if (_categoryService != null)
            return _categoryService;

        _categoryService = new CategoryService(
            _unitOfWorkMock.UnitOfWorkInstance,
            _mapper);
        return _categoryService;
    }

    public ICompanyService GetCompanyService()
    {
        if (_companyService != null)
            return _companyService;

        _companyService = new CompanyService(
            _unitOfWorkMock.UnitOfWorkInstance,
            GetOfferService(),
            GetImageService(),
            _mapper,
            GetAuthenticationService());
        return _companyService;
    }

    public IOfferService GetOfferService()
    {
        if (_offerService != null)
            return _offerService;

        _offerService = new OfferService(
            _unitOfWorkMock.UnitOfWorkInstance,
            _mapper,
            GetPurchaseService());
        return _offerService;
    }

    public IProductService GetProductService()
    {
        if (_productService != null)
            return _productService;

        _productService = new ProductService(
            _unitOfWorkMock.UnitOfWorkInstance,
            GetImageService(),
            _mapper);
        return _productService;
    }

    public IPurchaseService GetPurchaseService()
    {
        if (_purchaseService != null)
            return _purchaseService;

        _purchaseService = new PurchaseService(
            _unitOfWorkMock.UnitOfWorkInstance,
            _mapper);
        return _purchaseService;
    }

    public IImageService GetImageService()
    {
        return _imageService!;
    }

    public IAuthenticationService GetAuthenticationService()
    {
        var authServiceMock = new Mock<IAuthenticationService>();
        return authServiceMock.Object;
    }

    #endregion
}