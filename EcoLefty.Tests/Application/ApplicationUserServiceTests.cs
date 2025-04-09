using AutoMapper;
using EcoLefty.Application;
using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.ApplicationUsers;
using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.Authentication;
using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Application.Common.Images;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Contracts;
using EcoLefty.Tests.Mocks;
using Moq;

namespace EcoLefty.Tests.Application
{
    public class ApplicationUserServiceTests
    {
        private readonly UnitOfWorkMock _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly ApplicationUserService _userService;
        private readonly Mock<ITransactionWrapper> _transactionWrapperMock;
        private readonly Mock<IAuthenticationService> _authenticationServiceMock;
        private readonly Mock<IImageService> _imageServiceMock;

        public ApplicationUserServiceTests()
        {
            _unitOfWorkMock = new UnitOfWorkMock();

            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = mappingConfig.CreateMapper();

            _transactionWrapperMock = new Mock<ITransactionWrapper>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _imageServiceMock = new Mock<IImageService>();

            _transactionWrapperMock
                .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _transactionWrapperMock
                .Setup(t => t.CommitTransactionAsync())
                .Returns(Task.CompletedTask);
            _transactionWrapperMock
                .Setup(t => t.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            _userService = new ApplicationUserService(
                _unitOfWorkMock.UnitOfWorkInstance,
                _transactionWrapperMock.Object,
                _mapper,
                _imageServiceMock.Object,
                _authenticationServiceMock.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllApplicationUsers()
        {
            // Arrange
            var expectedCount = _unitOfWorkMock.GeneratedUsers.Count;

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsUserDetails()
        {
            // Arrange
            var expectedUser = _unitOfWorkMock.GeneratedUsers.First();
            var userId = expectedUser.Id;

            // Act
            var result = await _userService.GetByIdAsync(userId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsApplicationUserNotFoundException()
        {
            // Arrange
            var invalidId = "nonexistent-user";

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationUserNotFoundException>(() =>
                _userService.GetByIdAsync(invalidId, CancellationToken.None));
        }

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesApplicationUserAndReturnsToken()
        {
            // Arrange
            var createDto = new CreateApplicationUserRequestDto
            {
                FirstName = "Test User",
                Email = "test@example.com",
                BirthYear = 2005,
                BirthMonth = 2,
                BirthDay = 1
            };

            var tokenResponse = new TokenResponseDto
            {
                AccessToken = "dummyAccessToken",
                RefreshToken = "dummyRefreshToken"
            };

            _authenticationServiceMock
                .Setup(a => a.RegisterAccountAsync(It.IsAny<RegisterAccountRequestDto>(), AccountRole.User))
                .ReturnsAsync(tokenResponse);
            _authenticationServiceMock
                .Setup(a => a.GetAccountIdFromJwtTokenAsync(It.IsAny<string>()))
                .ReturnsAsync("newUserId");

            var initialCount = _unitOfWorkMock.GeneratedUsers.Count;

            // Act
            var result = await _userService.CreateAsync(createDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tokenResponse.AccessToken, result.AccessToken);
            Assert.Equal(initialCount + 1, _unitOfWorkMock.GeneratedUsers.Count);

            // Verify that the transaction was committed.
            _transactionWrapperMock.Verify(t => t.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenExceptionOccurs_RollsBackTransactionAndThrows()
        {
            // Arrange
            var createDto = new CreateApplicationUserRequestDto
            {
                FirstName = "Test User",
                Email = "test@example.com"
            };

            _authenticationServiceMock
                .Setup(a => a.RegisterAccountAsync(It.IsAny<RegisterAccountRequestDto>(), AccountRole.User))
                .ThrowsAsync(new Exception("Registration Failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _userService.CreateAsync(createDto, CancellationToken.None);
            });

            // Verify that the transaction was rolled back.
            _transactionWrapperMock.Verify(t => t.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesUserAndReturnsUpdatedResponse()
        {
            // Arrange
            var existingUser = _unitOfWorkMock.GeneratedUsers.First();
            var userId = existingUser.Id;

            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns(existingUser.Id);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            var updateDto = new UpdateApplicationUserRequestDto
            {
                FirstName = "Updated Name"
            };

            // Act
            var result = await _userService.UpdateAsync(userId, updateDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingUser.Id, result.Id);
            Assert.Equal(updateDto.FirstName, result.FirstName);
        }

        [Fact]
        public async Task UpdateAsync_WithNonexistentUser_ThrowsApplicationUserNotFoundException()
        {
            // Arrange
            var invalidUserId = "nonexistent-user";
            var updateDto = new UpdateApplicationUserRequestDto
            {
                FirstName = "Updated Name"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationUserNotFoundException>(async () =>
            {
                await _userService.UpdateAsync(invalidUserId, updateDto, CancellationToken.None);
            });
        }

        [Fact]
        public async Task UpdateAsync_WithForbiddenUser_ThrowsForbiddenException()
        {
            var existingUser = _unitOfWorkMock.GeneratedUsers.First();
            var differentUserId = "different-user-id";

            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns(differentUserId);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            var updateDto = new UpdateApplicationUserRequestDto
            {
                FirstName = "Updated Name"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(async () =>
            {
                await _userService.UpdateAsync(existingUser.Id, updateDto, CancellationToken.None);
            });
        }

        [Fact]
        public async Task DeleteAsync_WithAdminOrOwner_DeletesUserSuccessfully()
        {
            // Arrange
            var userToDelete = _unitOfWorkMock.GeneratedUsers.First();
            var userId = userToDelete.Id;
            var initialCount = _unitOfWorkMock.GeneratedUsers.Count;

            var mockUserContext = new Mock<ICurrentUserContext>();

            // Owner scenario
            mockUserContext.Setup(c => c.UserId).Returns(userId);
            mockUserContext.Setup(c => c.IsInRole("Admin")).Returns(false);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            var accountsMock = Mock.Get(_unitOfWorkMock.UnitOfWorkInstance.Accounts);
            accountsMock.Setup(a => a.DeactivateAsync(userId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.DeleteAsync(userId, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(initialCount - 1, _unitOfWorkMock.GeneratedUsers.Count);
        }

        [Fact]
        public async Task DeleteAsync_WithNonexistentUser_ThrowsApplicationUserNotFoundException()
        {
            // Arrange
            var invalidUserId = "nonexistent-user";

            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns("admin-user");
            mockUserContext.Setup(c => c.IsInRole("Admin")).Returns(true);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationUserNotFoundException>(async () =>
            {
                await _userService.DeleteAsync(invalidUserId, CancellationToken.None);
            });
        }

        [Fact]
        public async Task DeleteAsync_WithForbiddenUser_ThrowsForbiddenException()
        {
            // Arrange
            var userToDelete = _unitOfWorkMock.GeneratedUsers.First();
            var userId = userToDelete.Id;

            // Not admin and not owner).
            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns("some-other-user");
            mockUserContext.Setup(c => c.IsInRole("Admin")).Returns(false);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(async () =>
            {
                await _userService.DeleteAsync(userId, CancellationToken.None);
            });
        }
    }
}
