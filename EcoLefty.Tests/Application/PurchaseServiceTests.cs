using AutoMapper;
using EcoLefty.Application;
using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.Offers.DTOs;
using EcoLefty.Application.Purchases;
using EcoLefty.Application.Purchases.DTOs;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;
using EcoLefty.Tests.Mocks;
using Moq;

namespace EcoLefty.Tests.Application;

public class PurchaseServiceTests
{
    private readonly UnitOfWorkMock _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly PurchaseService _purchaseService;

    public PurchaseServiceTests()
    {
        _unitOfWorkMock = new UnitOfWorkMock();

        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = mappingConfig.CreateMapper();
        _purchaseService = new PurchaseService(_unitOfWorkMock.UnitOfWorkInstance, _mapper);
    }


    [Fact]
    public async Task GetAllAsync_ReturnsAllPurchases()
    {
        // Arrange
        var expectedPurchases = _unitOfWorkMock.GeneratedPurchases;

        // Act
        var result = await _purchaseService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPurchases.Count, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsPurchase()
    {
        // Arrange
        var expectedPurchase = _unitOfWorkMock.GeneratedPurchases.First();
        var purchaseId = expectedPurchase.Id;

        // Act
        var result = await _purchaseService.GetByIdAsync(purchaseId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(purchaseId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsPurchaseNotFoundException()
    {
        // Arrange
        var invalidId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<PurchaseNotFoundException>(() =>
            _purchaseService.GetByIdAsync(invalidId));
    }

    [Fact]
    public async Task GetPurchasesByOfferAsync_ReturnsCorrectPurchases()
    {
        // Arrange
        var offer = _unitOfWorkMock.GeneratedOffers.First();
        var expectedPurchases = _unitOfWorkMock.GeneratedPurchases
            .Where(p => p.OfferId == offer.Id);

        // Act
        var result = await _purchaseService.GetPurchasesByOfferAsync(offer.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPurchases.Count(), result.Count());
        Assert.All(result, r => Assert.Equal(offer.Id, r.Offer.Id));
    }

    [Fact]
    public async Task GetPurchasesByCustomerAsync_ReturnsCorrectPurchases()
    {
        // Arrange
        var customerId = _unitOfWorkMock.GeneratedUsers.First().Id;
        var expectedPurchases = _unitOfWorkMock.GeneratedPurchases
            .Where(p => p.CustomerId == customerId);

        // Act
        var result = await _purchaseService.GetPurchasesByCustomerAsync(customerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPurchases.Count(), result.Count());
        Assert.All(result, r => Assert.Equal(customerId, r.Customer.Id));
    }

    [Fact]
    public async Task CreatePurchaseAsync_WithValidData_CreatesPurchaseAndUpdatesBalances()
    {
        // Arrange
        var user = _unitOfWorkMock.GeneratedUsers.First();
        var offer = _unitOfWorkMock.GeneratedOffers.First();
        var company = _unitOfWorkMock.GeneratedCompanies.First(c => c.Id == offer.Product.CompanyId);

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(user.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Setup starting balances
        user.Balance = 5000m;
        company.Balance = 10000m;
        offer.QuantityAvailable = 10;

        var createPurchaseDto = new CreatePurchaseRequestDto
        {
            OfferId = offer.Id,
            Quantity = 2
        };

        // Expected values
        var expectedPurchasePrice = offer.UnitPrice * createPurchaseDto.Quantity;
        var expectedUserBalance = user.Balance - expectedPurchasePrice;
        var expectedCompanyBalance = company.Balance + expectedPurchasePrice;
        var expectedOfferQuantity = offer.QuantityAvailable - createPurchaseDto.Quantity;
        var initialPurchaseCount = _unitOfWorkMock.GeneratedPurchases.Count;

        // Act
        var result = await _purchaseService.CreatePurchaseAsync(createPurchaseDto);
        result.Customer = _mapper.Map<ApplicationUserResponseDto>(user);
        result.Offer = _mapper.Map<OfferDetailsResponseDto>(offer);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(offer.Id, result.Offer.Id);
        Assert.Equal(createPurchaseDto.Quantity, result.Quantity);
        Assert.Equal(expectedPurchasePrice, result.TotalPrice);
        Assert.Equal(user.Id, result.Customer.Id);

        // Check balances were updated
        Assert.Equal(expectedUserBalance, user.Balance);
        Assert.Equal(expectedCompanyBalance, company.Balance);
        Assert.Equal(expectedOfferQuantity, offer.QuantityAvailable);

        // Check a purchase was added
        Assert.Equal(initialPurchaseCount + 1, _unitOfWorkMock.GeneratedPurchases.Count);
    }

    [Fact]
    public async Task CreatePurchaseAsync_WithInsufficientOfferQuantity_ThrowsBadRequestException()
    {
        // Arrange
        var user = _unitOfWorkMock.GeneratedUsers.First();
        var offer = _unitOfWorkMock.GeneratedOffers.First();

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(user.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Setup insufficient quantity
        user.Balance = 1000m;
        offer.QuantityAvailable = 1;

        var createPurchaseDto = new CreatePurchaseRequestDto
        {
            OfferId = offer.Id,
            Quantity = 2
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _purchaseService.CreatePurchaseAsync(createPurchaseDto));
    }

    [Fact]
    public async Task CreatePurchaseAsync_WithInsufficientBalance_ThrowsBadRequestException()
    {
        // Arrange
        var user = _unitOfWorkMock.GeneratedUsers.First();
        var offer = _unitOfWorkMock.GeneratedOffers.First();

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(user.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Setup insufficient balance
        user.Balance = 10m;
        offer.UnitPrice = 100m;
        offer.QuantityAvailable = 10;

        var createPurchaseDto = new CreatePurchaseRequestDto
        {
            OfferId = offer.Id,
            Quantity = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _purchaseService.CreatePurchaseAsync(createPurchaseDto));
    }

    [Fact]
    public async Task CancelPurchaseAsync_WithValidRecentPurchase_CancelsPurchaseAndReversesBalances()
    {
        // Arrange
        var purchase = _unitOfWorkMock.GeneratedPurchases.First();
        var user = _unitOfWorkMock.GeneratedUsers.First(u => u.Id == purchase.CustomerId);
        var offer = _unitOfWorkMock.GeneratedOffers.First(o => o.Id == purchase.OfferId);
        var company = _unitOfWorkMock.GeneratedCompanies.First(c => c.Id == offer.Product.CompanyId);

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(user.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Setup initial state
        var initialUserBalance = user.Balance;
        var initialCompanyBalance = company.Balance;
        purchase.PurchaseStatus = PurchaseStatus.Active;
        purchase.PurchaseDateUtc = DateTime.UtcNow; // Recent purchase

        // Expected values
        var expectedUserBalance = initialUserBalance + purchase.TotalPrice;
        var expectedCompanyBalance = initialCompanyBalance - purchase.TotalPrice;

        // Act
        var result = await _purchaseService.CancelPurchaseAsync(purchase.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(PurchaseStatus.Cancelled, purchase.PurchaseStatus);
        Assert.Equal(expectedUserBalance, user.Balance);
        Assert.Equal(expectedCompanyBalance, company.Balance);
    }

    [Fact]
    public async Task CancelPurchaseAsync_WithOldPurchase_ThrowsInvalidOperationException()
    {
        // Arrange
        var purchase = _unitOfWorkMock.GeneratedPurchases.First();
        var user = _unitOfWorkMock.GeneratedUsers.First(u => u.Id == purchase.CustomerId);

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(user.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Setup old purchase
        purchase.PurchaseStatus = PurchaseStatus.Active;
        purchase.PurchaseDateUtc = DateTime.UtcNow.AddHours(-1); // Old purchase

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _purchaseService.CancelPurchaseAsync(purchase.Id));
    }

    [Fact]
    public async Task CancelPurchaseAsync_WithDifferentUser_ThrowsForbiddenException()
    {
        // Arrange
        var purchase = _unitOfWorkMock.GeneratedPurchases.First();
        var differentUser = _unitOfWorkMock.GeneratedUsers.First(u => u.Id != purchase.CustomerId);

        // Set different user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(differentUser.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        purchase.PurchaseStatus = PurchaseStatus.Active;
        purchase.PurchaseDateUtc = DateTime.UtcNow; // Recent purchase

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _purchaseService.CancelPurchaseAsync(purchase.Id));
    }

    [Fact]
    public async Task CancelPurchaseAsync_WithAlreadyCancelledPurchase_ThrowsBadRequestException()
    {
        // Arrange
        var purchase = _unitOfWorkMock.GeneratedPurchases.First();
        var user = _unitOfWorkMock.GeneratedUsers.First(u => u.Id == purchase.CustomerId);

        // Set current user
        var mockUserContext = new Mock<ICurrentUserContext>();
        mockUserContext.Setup(c => c.UserId).Returns(user.Id);
        Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
            .Setup(u => u.CurrentUserContext)
            .Returns(mockUserContext.Object);

        // Setup already cancelled purchase
        purchase.PurchaseStatus = PurchaseStatus.Cancelled;
        purchase.PurchaseDateUtc = DateTime.UtcNow; // Recent purchase

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _purchaseService.CancelPurchaseAsync(purchase.Id));
    }

    [Fact]
    public async Task CancelAllPurchasesByOfferAsync_CancelsActivePurchasesForOffer()
    {
        // Arrange
        var offer = _unitOfWorkMock.GeneratedOffers.First();
        var offerPurchases = _unitOfWorkMock.GeneratedPurchases
            .Where(p => p.OfferId == offer.Id)
            .ToList();

        // Make all purchases active
        foreach (var purchase in offerPurchases)
        {
            purchase.PurchaseStatus = PurchaseStatus.Active;
        }

        // Expected
        var initialActivePurchaseCount = offerPurchases.Count(p => p.PurchaseStatus == PurchaseStatus.Active);

        // Act
        var result = await _purchaseService.CancelAllPurchasesByOfferAsync(offer.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(0, offerPurchases.Count(p => p.PurchaseStatus == PurchaseStatus.Active));
        Assert.Equal(initialActivePurchaseCount, offerPurchases.Count(p => p.PurchaseStatus == PurchaseStatus.Cancelled));
    }

    [Fact]
    public async Task CancelAllPurchasesByCustomerAsync_CancelsActivePurchasesForCustomer()
    {
        // Arrange
        var customer = _unitOfWorkMock.GeneratedUsers.First();
        var customerPurchases = _unitOfWorkMock.GeneratedPurchases
            .Where(p => p.CustomerId == customer.Id)
            .ToList();

        // Make sure we have at least 2 purchases for the customer
        if (customerPurchases.Count < 2)
        {
            var offer = _unitOfWorkMock.GeneratedOffers.First();
            var newPurchase = new Purchase
            {
                Id = _unitOfWorkMock.GeneratedPurchases.Max(p => p.Id) + 1,
                CustomerId = customer.Id,
                OfferId = offer.Id,
                Quantity = 1,
                TotalPrice = offer.UnitPrice,
                PurchaseDateUtc = DateTime.UtcNow,
                PurchaseStatus = PurchaseStatus.Active
            };
            _unitOfWorkMock.GeneratedPurchases.Add(newPurchase);
            customerPurchases.Add(newPurchase);
        }

        // Make all purchases active
        foreach (var purchase in customerPurchases)
        {
            purchase.PurchaseStatus = PurchaseStatus.Active;
        }

        // Expected
        var initialActivePurchaseCount = customerPurchases.Count(p => p.PurchaseStatus == PurchaseStatus.Active);

        // Act
        var result = await _purchaseService.CancelAllPurchasesByCustomerAsync(customer.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(0, customerPurchases.Count(p => p.PurchaseStatus == PurchaseStatus.Active));
        Assert.Equal(initialActivePurchaseCount, customerPurchases.Count(p => p.PurchaseStatus == PurchaseStatus.Cancelled));
    }
}