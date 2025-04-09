using AutoMapper;
using EcoLefty.Application;
using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.Authentication;
using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Application.Common.Images;
using EcoLefty.Application.Companies;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Application.Offers;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities.Identity;
using EcoLefty.Tests.Mocks;
using Moq;

namespace EcoLefty.Tests.Application
{
    public class CompanyServiceTests
    {
        private readonly UnitOfWorkMock _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly CompanyService _companyService;
        private readonly Mock<ITransactionWrapper> _transactionWrapperMock;
        private readonly Mock<IOfferService> _offerServiceMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly Mock<IAuthenticationService> _authenticationServiceMock;

        public CompanyServiceTests()
        {
            _unitOfWorkMock = new UnitOfWorkMock();

            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = mappingConfig.CreateMapper();

            _transactionWrapperMock = new Mock<ITransactionWrapper>();
            _offerServiceMock = new Mock<IOfferService>();
            _imageServiceMock = new Mock<IImageService>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();

            _transactionWrapperMock
                .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _transactionWrapperMock
                .Setup(t => t.CommitTransactionAsync())
                .Returns(Task.CompletedTask);
            _transactionWrapperMock
                .Setup(t => t.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            _companyService = new CompanyService(
                _unitOfWorkMock.UnitOfWorkInstance,
                _transactionWrapperMock.Object,
                _offerServiceMock.Object,
                _imageServiceMock.Object,
                _mapper,
                _authenticationServiceMock.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCompanies()
        {
            // Arrange
            var expectedCount = _unitOfWorkMock.GeneratedCompanies.Count;

            // Act
            var result = await _companyService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCompanyDetails()
        {
            // Arrange
            var company = _unitOfWorkMock.GeneratedCompanies.First();

            var dummyOffers = new List<object>(); // replace object with your OfferResponseDto if available

            // Act
            var result = await _companyService.GetByIdAsync(company.Id, CancellationToken.None, includeArchivedOffers: false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(company.Id, result.Id);
            Assert.Equal(dummyOffers, result.Offers);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsCompanyNotFoundException()
        {
            // Arrange
            var invalidId = "nonexistent-company-id";

            // Act & Assert
            await Assert.ThrowsAsync<CompanyNotFoundException>(async () =>
            {
                await _companyService.GetByIdAsync(invalidId, CancellationToken.None);
            });
        }

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesCompanyAndReturnsToken()
        {
            // Arrange
            var createDto = new CreateCompanyRequestDto
            {
                Name = "Test Company",
                Email = "company@test.com"
            };

            var tokenResponse = new TokenResponseDto
            {
                AccessToken = "dummyAccessToken",
                RefreshToken = "dummyRefreshToken"
            };

            _authenticationServiceMock
                .Setup(a => a.RegisterAccountAsync(It.IsAny<RegisterAccountRequestDto>(), AccountRole.Company))
                .ReturnsAsync(tokenResponse);
            _authenticationServiceMock
                .Setup(a => a.GetAccountIdFromJwtTokenAsync(It.IsAny<string>()))
                .ReturnsAsync("newCompanyId");

            var initialCount = _unitOfWorkMock.GeneratedCompanies.Count;

            // Act
            var result = await _companyService.CreateAsync(createDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tokenResponse.AccessToken, result.AccessToken);
            Assert.Equal(initialCount + 1, _unitOfWorkMock.GeneratedCompanies.Count);
            _transactionWrapperMock.Verify(t => t.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenExceptionOccurs_RollsBackTransactionAndThrows()
        {
            // Arrange
            var createDto = new CreateCompanyRequestDto
            {
                Name = "Test Company",
                Email = "company@test.com"
            };

            _authenticationServiceMock
                .Setup(a => a.RegisterAccountAsync(It.IsAny<RegisterAccountRequestDto>(), AccountRole.Company))
                .ThrowsAsync(new Exception("Registration Failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _companyService.CreateAsync(createDto, CancellationToken.None);
            });

            // Verify that a rollback occurred.
            _transactionWrapperMock.Verify(t => t.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesCompanyAndReturnsUpdatedResponse()
        {
            // Arrange
            var company = _unitOfWorkMock.GeneratedCompanies.First();
            var companyId = company.Id;

            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns(companyId);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            var updateDto = new UpdateCompanyRequestDto
            {
                Name = "Updated Company Name"
            };

            // Act
            var result = await _companyService.UpdateAsync(companyId, updateDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(companyId, result.Id);
            Assert.Equal(updateDto.Name, result.Name);
        }

        [Fact]
        public async Task UpdateAsync_WithNonexistentCompany_ThrowsCompanyNotFoundException()
        {
            // Arrange
            var invalidId = "nonexistent-company-id";
            var updateDto = new UpdateCompanyRequestDto
            {
                Name = "Updated Name"
            };

            // Act & Assert
            await Assert.ThrowsAsync<CompanyNotFoundException>(async () =>
            {
                await _companyService.UpdateAsync(invalidId, updateDto, CancellationToken.None);
            });
        }

        [Fact]
        public async Task UpdateAsync_WithForbiddenUser_ThrowsForbiddenException()
        {
            var company = _unitOfWorkMock.GeneratedCompanies.First();
            var companyId = company.Id;

            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns("different-user-id");
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            var updateDto = new UpdateCompanyRequestDto
            {
                Name = "Updated Name"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(async () =>
            {
                await _companyService.UpdateAsync(companyId, updateDto, CancellationToken.None);
            });
        }

        [Fact]
        public async Task ApproveCompanyAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var company = _unitOfWorkMock.GeneratedCompanies.First();
            var companyId = company.Id;
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance.Accounts)
                .Setup(a => a.GetByIdAsync(companyId, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Account { Id = companyId });

            // Act
            var result = await _companyService.ApproveCompanyAsync(companyId, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.True(company.IsApproved);
            _authenticationServiceMock.Verify(a => a.AddClaimAsync(It.IsAny<Account>(), "IsApproved", "True"), Times.Once);
        }

        [Fact]
        public async Task ApproveCompanyAsync_WithInvalidId_ThrowsCompanyNotFoundException()
        {
            // Arrange
            var invalidId = "invalid-company-id";

            // Act & Assert
            await Assert.ThrowsAsync<CompanyNotFoundException>(async () =>
            {
                await _companyService.ApproveCompanyAsync(invalidId, CancellationToken.None);
            });
        }

        [Fact]
        public async Task DeleteAsync_WithAdminOrOwner_DeletesCompanySuccessfully()
        {
            // Arrange
            var company = _unitOfWorkMock.GeneratedCompanies.First();
            var companyId = company.Id;
            var initialCount = _unitOfWorkMock.GeneratedCompanies.Count;

            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns(companyId);
            mockUserContext.Setup(c => c.IsInRole("Admin")).Returns(false);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            var accountsMock = Mock.Get(_unitOfWorkMock.UnitOfWorkInstance.Accounts);
            accountsMock.Setup(a => a.DeactivateAsync(companyId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _companyService.DeleteAsync(companyId, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(initialCount - 1, _unitOfWorkMock.GeneratedCompanies.Count);
        }

        [Fact]
        public async Task DeleteAsync_WithNonexistentCompany_ThrowsCompanyNotFoundException()
        {
            // Arrange
            var invalidId = "nonexistent-company-id";

            // Act & Assert
            await Assert.ThrowsAsync<CompanyNotFoundException>(async () =>
            {
                await _companyService.DeleteAsync(invalidId, CancellationToken.None);
            });
        }

        [Fact]
        public async Task DeleteAsync_WithForbiddenUser_ThrowsUnauthorizedException()
        {
            // Arrange
            var company = _unitOfWorkMock.GeneratedCompanies.First();
            var companyId = company.Id;

            // Neither admin nor owner
            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns("another-user");
            mockUserContext.Setup(c => c.IsInRole("Admin")).Returns(false);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () =>
            {
                await _companyService.DeleteAsync(companyId, CancellationToken.None);
            });
        }
    }
}
