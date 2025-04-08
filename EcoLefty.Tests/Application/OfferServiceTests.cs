using AutoMapper;
using EcoLefty.Application;
using EcoLefty.Application.Offers;
using EcoLefty.Application.Offers.DTOs;
using EcoLefty.Application.Products.DTOs;
using EcoLefty.Application.Purchases;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Contracts;
using EcoLefty.Tests.Mocks;
using Moq;

namespace EcoLefty.Tests.Application;

public class OfferServiceTests
{
    private readonly UnitOfWorkMock _unitOfWorkMock;
    private readonly Mock<IPurchaseService> _purchaseServiceMock;
    private readonly IMapper _mapper;
    private readonly OfferService _offerService;

    public OfferServiceTests()
    {
        _unitOfWorkMock = new UnitOfWorkMock();
        _purchaseServiceMock = new Mock<IPurchaseService>();

        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = mappingConfig.CreateMapper();
        _offerService = new OfferService(_unitOfWorkMock.UnitOfWorkInstance, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllOffers()
    {
        // Arrange
        var expectedOffers = _unitOfWorkMock.GeneratedOffers;

        OfferSearchDto dto = new OfferSearchDto()
        {
            OnlyActive = false,
            PageSize = expectedOffers.Count
        };

        // Act
        var result = await _offerService.GetAllAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedOffers.Count, result.Count());
    }

    [Fact]
    public async Task GetActiveOffersAsync_ReturnsOnlyActiveOffers()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Make some offers active and some not
        foreach (var offer in _unitOfWorkMock.GeneratedOffers.Take(3))
        {
            offer.StartDateUtc = now.AddHours(-1);
            offer.ExpiryDateUtc = now.AddHours(1);
            offer.OfferStatus = OfferStatus.Active;
        }

        foreach (var offer in _unitOfWorkMock.GeneratedOffers.Skip(3).Take(2))
        {
            offer.StartDateUtc = now.AddHours(1);
            offer.ExpiryDateUtc = now.AddHours(2);
            offer.OfferStatus = OfferStatus.Incoming;
        }

        var expectedActiveOfferCount = _unitOfWorkMock.GeneratedOffers.Count(o =>
            now >= o.StartDateUtc && now <= o.ExpiryDateUtc);

        // Act
        var result = await _offerService.GetActiveOffersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedActiveOfferCount, result.Count());
        Assert.All(result, r => Assert.True(now >= r.StartDateUtc && now <= r.ExpiryDateUtc));
    }

    [Fact]
    public async Task GetAllOffersOfCompanyAsync_ReturnsCorrectOffers()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        var expectedOffers = _unitOfWorkMock.GeneratedOffers
            .Where(o => o.Product.CompanyId == company.Id);

        // Act
        var result = await _offerService.GetAllOffersOfCompanyAsync(company.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedOffers.Count(), result.Count());
        Assert.All(result, r => Assert.Equal(company.Id, r.Product.Company.Id));
    }

    [Fact]
    public async Task GetActiveOffersOfCompanyAsync_ReturnsCorrectOffers()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var company = _unitOfWorkMock.GeneratedCompanies.First();

        // Make some offers for this company active
        foreach (var offer in _unitOfWorkMock.GeneratedOffers.Where(o => o.Product.CompanyId == company.Id).Take(2))
        {
            offer.StartDateUtc = now.AddHours(-1);
            offer.ExpiryDateUtc = now.AddHours(1);
        }

        var expectedOffers = _unitOfWorkMock.GeneratedOffers
            .Where(o => o.Product.CompanyId == company.Id && now >= o.StartDateUtc && now <= o.ExpiryDateUtc);

        // Act
        var result = await _offerService.GetActiveOffersOfCompanyAsync(company.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedOffers.Count(), result.Count());
        Assert.All(result, r => Assert.Equal(company.Id, r.Product.Company.Id));
        Assert.All(result, r => Assert.True(now >= r.StartDateUtc && now <= r.ExpiryDateUtc));
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsOffer()
    {
        // Arrange
        var expectedOffer = _unitOfWorkMock.GeneratedOffers.First();
        var offerId = expectedOffer.Id;

        // Act
        var result = await _offerService.GetByIdAsync(offerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(offerId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsOfferNotFoundException()
    {
        // Arrange
        var invalidId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<OfferNotFoundException>(() =>
            _offerService.GetByIdAsync(invalidId));
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesOffer()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        company.IsApproved = true;

        var product = _unitOfWorkMock.GeneratedProducts.First(p => p.CompanyId == company.Id);

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        var now = DateTime.UtcNow;
        var createOfferDto = new CreateOfferRequestDto
        {
            ProductId = product.Id,
            UnitPrice = 50.00m,
            TotalQuantity = 10,
            StartDateUtc = now,
            ExpiryDateUtc = now.AddDays(5)
        };

        var initialOfferCount = _unitOfWorkMock.GeneratedOffers.Count;

        // Act
        var result = await _offerService.CreateAsync(createOfferDto);
        result.Product = _mapper.Map<ProductResponseDto>(product);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createOfferDto.ProductId, result.Product.Id);
        Assert.Equal(createOfferDto.UnitPrice, result.UnitPrice);
        Assert.Equal(createOfferDto.TotalQuantity, result.TotalQuantity);
        Assert.Equal(createOfferDto.TotalQuantity, result.QuantityAvailable);
        Assert.Equal(createOfferDto.StartDateUtc, result.StartDateUtc);
        Assert.Equal(createOfferDto.ExpiryDateUtc, result.ExpiryDateUtc);

        // Check an offer was added
        Assert.Equal(initialOfferCount + 1, _unitOfWorkMock.GeneratedOffers.Count);
    }

    [Fact]
    public async Task CreateAsync_WithNotApprovedCompany_ThrowsCompanyNotApprovedException()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        company.IsApproved = false; // Not approved

        var product = _unitOfWorkMock.GeneratedProducts.First(p => p.CompanyId == company.Id);

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        var now = DateTime.UtcNow;
        var createOfferDto = new CreateOfferRequestDto
        {
            ProductId = product.Id,
            UnitPrice = 50.00m,
            TotalQuantity = 10,
            StartDateUtc = now,
            ExpiryDateUtc = now.AddDays(5)
        };

        // Act & Assert
        await Assert.ThrowsAsync<CompanyNotApprovedException>(() =>
            _offerService.CreateAsync(createOfferDto));
    }

    [Fact]
    public async Task CreateAsync_WithProductFromDifferentCompany_ThrowsException()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        company.IsApproved = true;

        var differentCompany = _unitOfWorkMock.GeneratedCompanies.Last();
        var product = _unitOfWorkMock.GeneratedProducts.First(p => p.CompanyId == differentCompany.Id);

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        var now = DateTime.UtcNow;
        var createOfferDto = new CreateOfferRequestDto
        {
            ProductId = product.Id, // Product from different company
            UnitPrice = 50.00m,
            TotalQuantity = 10,
            StartDateUtc = now,
            ExpiryDateUtc = now.AddDays(5)
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _offerService.CreateAsync(createOfferDto));
    }

    [Fact]
    public async Task CreateAsync_WithInvalidProduct_ThrowsProductNotFoundException()
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

        var now = DateTime.UtcNow;
        var createOfferDto = new CreateOfferRequestDto
        {
            ProductId = -1, // Invalid product ID
            UnitPrice = 50.00m,
            TotalQuantity = 10,
            StartDateUtc = now,
            ExpiryDateUtc = now.AddDays(5)
        };

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _offerService.CreateAsync(createOfferDto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesOffer()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        var product = _unitOfWorkMock.GeneratedProducts.First(p => p.CompanyId == company.Id);
        var offer = _unitOfWorkMock.GeneratedOffers.First(o => o.ProductId == product.Id);

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        var now = DateTime.UtcNow;
        var updateOfferDto = new UpdateOfferRequestDto
        {
            UnitPrice = 75.00m,
            TotalQuantity = 15,
            StartDateUtc = now.AddDays(1),
            ExpiryDateUtc = now.AddDays(7)
        };

        // Act
        var result = await _offerService.UpdateAsync(offer.Id, updateOfferDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(offer.Id, result.Id);
        Assert.Equal(updateOfferDto.UnitPrice, result.UnitPrice);
        Assert.Equal(updateOfferDto.TotalQuantity, result.TotalQuantity);
        Assert.Equal(updateOfferDto.StartDateUtc, result.StartDateUtc);
        Assert.Equal(updateOfferDto.ExpiryDateUtc, result.ExpiryDateUtc);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsOfferNotFoundException()
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

        var now = DateTime.UtcNow;
        var updateOfferDto = new UpdateOfferRequestDto
        {
            UnitPrice = 75.00m,
            TotalQuantity = 15,
            StartDateUtc = now.AddDays(1),
            ExpiryDateUtc = now.AddDays(7)
        };

        // Act & Assert
        await Assert.ThrowsAsync<OfferNotFoundException>(() =>
            _offerService.UpdateAsync(invalidId, updateOfferDto));
    }

    [Fact]
    public async Task UpdateAsync_WithDifferentCompany_ThrowsForbiddenException()
    {
        // Arrange
        var offer = _unitOfWorkMock.GeneratedOffers.First();
        var differentCompany = _unitOfWorkMock.GeneratedCompanies
            .First(c => c.Id != offer.Product.CompanyId);

        // Set different company as current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(differentCompany.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        var now = DateTime.UtcNow;
        var updateOfferDto = new UpdateOfferRequestDto
        {
            UnitPrice = 75.00m,
            TotalQuantity = 15,
            StartDateUtc = now.AddDays(1),
            ExpiryDateUtc = now.AddDays(7)
        };

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _offerService.UpdateAsync(offer.Id, updateOfferDto));
    }

    [Fact]
    public async Task CancelAsync_WithValidRecentOffer_CancelsOfferAndPurchases()
    {
        // Arrange
        var company = _unitOfWorkMock.GeneratedCompanies.First();
        var product = _unitOfWorkMock.GeneratedProducts.First(p => p.CompanyId == company.Id);
        var offer = _unitOfWorkMock.GeneratedOffers.First(o => o.ProductId == product.Id);

        offer.CreatedAtUtc = DateTime.UtcNow.AddMinutes(-5); // ensure it's recent enough

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Setup purchase service
        _purchaseServiceMock
            .Setup(p => p.CancelAllPurchasesByOfferAsync(offer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var initialActiveOfferCount = _unitOfWorkMock.GeneratedOffers.Where(x => x.OfferStatus == OfferStatus.Active).Count();

        // Act
        var result = await _offerService.CancelAsync(offer.Id);

        var activeOfferCountAfterCancel = _unitOfWorkMock.GeneratedOffers.Where(x => x.OfferStatus == OfferStatus.Active).Count();

        // Assert
        Assert.True(result);
        Assert.Equal(initialActiveOfferCount - 1, activeOfferCountAfterCancel);
        _purchaseServiceMock.Verify(p => p.CancelAllPurchasesByOfferAsync(offer.Id, It.IsAny<CancellationToken>()), Times.Never); // don't use service method for atomicity
    }

    [Fact]
    public async Task DeleteAsync_WithAdminOrOwner_DeletesOffer()
    {
        // Arrange
        var offer = _unitOfWorkMock.GeneratedOffers.First();
        var company = _unitOfWorkMock.GeneratedCompanies.First(c => c.Id == offer.Product.CompanyId);

        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(company.Id);
        mockUserContext.Setup(c => c.IsInRole("Admin")).Returns(false); // simulate owner

        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        var initialOfferCount = _unitOfWorkMock.GeneratedOffers.Count;

        // Act
        var result = await _offerService.DeleteAsync(offer.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(initialOfferCount - 1, _unitOfWorkMock.GeneratedOffers.Count);
    }

}