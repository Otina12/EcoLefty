using AutoMapper;
using EcoLefty.Application;
using EcoLefty.Application.Common.Images;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Application.Products;
using EcoLefty.Application.Products.DTOs;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Contracts;
using EcoLefty.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Moq;

namespace EcoLefty.Tests.Application;

public class ProductServiceTests
{
    private readonly UnitOfWorkMock _unitOfWorkMock;
    private readonly Mock<IImageService> _imageServiceMock;
    private readonly IMapper _mapper;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new UnitOfWorkMock();
        _imageServiceMock = new Mock<IImageService>();

        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = mappingConfig.CreateMapper();
        _productService = new ProductService(_unitOfWorkMock.UnitOfWorkInstance, _imageServiceMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        var expectedProducts = _unitOfWorkMock.GeneratedProducts;

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProducts.Count, result.Count());
    }

    [Fact]
    public async Task GetAllProductsOfCompanyAsync_ReturnsCorrectProducts()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        var expectedProducts = _unitOfWorkMock.GeneratedProducts
            .Where(p => p.CompanyId == company.Id);

        // Act
        var result = await _productService.GetAllProductsOfCompanyAsync(company.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProducts.Count(), result.Count());
        Assert.All(result, r => Assert.Equal(company.Id, r.Company.Id));
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var expectedProduct = _unitOfWorkMock.GeneratedProducts.First();
        var productId = expectedProduct.Id;

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal(expectedProduct.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsProductNotFoundException()
    {
        // Arrange
        var invalidId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _productService.GetByIdAsync(invalidId));
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesProduct()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        company.IsApproved = true;

        var categories = _unitOfWorkMock.GeneratedCategories.Take(2).ToList();
        var categoryIds = categories.Select(c => c.Id).ToList();

        // Set current user (Company)
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Mock image upload
        var mockImageFile = new Mock<IFormFile>();
        _imageServiceMock.Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("test-image-url.jpg");

        var createProductDto = new CreateProductRequestDto
        {
            Name = "Test Product",
            Description = "Test Description",
            CategoryIds = categoryIds,
            ImageFile = mockImageFile.Object
        };

        var initialProductCount = _unitOfWorkMock.GeneratedProducts.Count;

        // Act
        var result = await _productService.CreateAsync(createProductDto);
        result.Company = _mapper.Map<CompanyResponseDto>(company);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createProductDto.Name, result.Name);
        Assert.Equal(createProductDto.Description, result.Description);
        Assert.Equal(company.Id, result.Company.Id);
        Assert.Equal("test-image-url.jpg", result.ImageUrl);

        // Check a product was added
        Assert.Equal(initialProductCount + 1, _unitOfWorkMock.GeneratedProducts.Count);
    }

    [Fact]
    public async Task CreateAsync_WithExistingName_ThrowsProductAlreadyExistsException()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        company.IsApproved = true;

        var existingProduct = _unitOfWorkMock.GeneratedProducts.First(p => p.CompanyId == company.Id);

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        var createProductDto = new CreateProductRequestDto
        {
            Name = existingProduct.Name, // Using existing name
            Description = "Test Description"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ProductAlreadyExistsException>(() =>
            _productService.CreateAsync(createProductDto));
    }

    [Fact]
    public async Task CreateAsync_WithNotApprovedCompany_ThrowsCompanyNotApprovedException()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        company.IsApproved = false; // Not approved company

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        var createProductDto = new CreateProductRequestDto
        {
            Name = "Test Product",
            Description = "Test Description"
        };

        // Act & Assert
        await Assert.ThrowsAsync<CompanyNotApprovedException>(() =>
            _productService.CreateAsync(createProductDto));
    }

    [Fact]
    public async Task CreateAsync_WithInvalidCategory_ThrowsCategoryNotFoundException()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        company.IsApproved = true;

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        var createProductDto = new CreateProductRequestDto
        {
            Name = "Test Product",
            Description = "Test Description",
            CategoryIds = new List<int> { -1 } // Invalid category ID
        };

        // Act & Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(() =>
            _productService.CreateAsync(createProductDto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesProduct()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        var product = _unitOfWorkMock.GeneratedProducts.First(p => p.CompanyId == company.Id);

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Mock image upload
        var mockImageFile = new Mock<IFormFile>();
        _imageServiceMock.Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("updated-image-url.jpg");

        var updateProductDto = new UpdateProductRequestDto
        {
            Name = "Updated Product Name",
            Description = "Updated Product Description",
            ImageFile = mockImageFile.Object
        };

        // Act
        var result = await _productService.UpdateAsync(product.Id, updateProductDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateProductDto.Name, result.Name);
        Assert.Equal(updateProductDto.Description, result.Description);
        Assert.Equal("updated-image-url.jpg", result.ImageUrl);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsProductNotFoundException()
    {
        // Arrange
        var invalidId = -1;
        var company = _unitOfWorkMock.GeneratedCompanies.First();

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        var updateProductDto = new UpdateProductRequestDto
        {
            Name = "Updated Product Name",
            Description = "Updated Product Description"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _productService.UpdateAsync(invalidId, updateProductDto));
    }

    [Fact]
    public async Task UpdateAsync_WithDifferentCompany_ThrowsForbiddenException()
    {
        // Arrange
        var product = _unitOfWorkMock.GeneratedProducts.First();
        var differentCompany = _unitOfWorkMock.GeneratedCompanies.First(c => c.Id != product.CompanyId);

        // Set different company as current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(differentCompany.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        var updateProductDto = new UpdateProductRequestDto
        {
            Name = "Updated Product Name",
            Description = "Updated Product Description"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _productService.UpdateAsync(product.Id, updateProductDto));
    }

    [Fact]
    public async Task DeleteAsync_AsOwner_DeletesProduct()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        var product = _unitOfWorkMock.GeneratedProducts.First(p => p.CompanyId == company.Id);

        // Set owner as current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        mockUserContext.Setup(c => c.IsInRole("Admin")).Returns(false);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Act
        var result = await _productService.DeleteAsync(product.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_AsAdmin_DeletesProduct()
    {
        // Arrange
        var product = _unitOfWorkMock.GeneratedProducts.First();
        var adminId = "admin-id"; // Different from product owner

        // Set admin as current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(adminId);
        mockUserContext.Setup(c => c.IsInRole("Admin")).Returns(true);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Act
        var result = await _productService.DeleteAsync(product.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_AsUnauthorizedUser_ThrowsForbiddenException()
    {
        // Arrange
        var product = _unitOfWorkMock.GeneratedProducts.First();
        var unauthorizedUserId = "unauthorized-id";

        // Set unauthorized user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(unauthorizedUserId);
        mockUserContext.Setup(c => c.IsInRole("Admin")).Returns(false);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _productService.DeleteAsync(product.Id));
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsProductNotFoundException()
    {
        // Arrange
        var invalidId = -1;
        var adminId = "admin-id";

        // Set admin as current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(adminId);
        mockUserContext.Setup(c => c.IsInRole("Admin")).Returns(true);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _productService.DeleteAsync(invalidId));
    }
}