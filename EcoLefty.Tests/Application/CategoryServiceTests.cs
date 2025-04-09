using AutoMapper;
using EcoLefty.Application;
using EcoLefty.Application.Categories;
using EcoLefty.Application.Categories.DTOs;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;
using EcoLefty.Tests.Mocks;
using Moq;

namespace EcoLefty.Tests.Application
{
    public class CategoryServiceTests
    {
        private readonly UnitOfWorkMock _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _unitOfWorkMock = new UnitOfWorkMock();

            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = mappingConfig.CreateMapper();

            _categoryService = new CategoryService(_unitOfWorkMock.UnitOfWorkInstance, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCategories()
        {
            // Arrange
            var expectedCount = _unitOfWorkMock.GeneratedCategories.Count;

            // Act
            var result = await _categoryService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetAllFollowedCategoriesByUserIdAsync_WithValidUser_ReturnsFollowedCategories()
        {
            // Arrange
            var user = _unitOfWorkMock.GeneratedUsers.First();

            if (user.FollowedCategories == null)
                user.FollowedCategories = new List<Category>();

            var category = _unitOfWorkMock.GeneratedCategories.First();
            user.FollowedCategories.Add(category);

            // Act
            var result = await _categoryService.GetAllFollowedCategoriesByUserIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, c => c.Id == category.Id);
        }

        [Fact]
        public async Task GetAllFollowedCategoriesByUserIdAsync_WithInvalidUser_ThrowsApplicationUserNotFoundException()
        {
            // Arrange
            var invalidUserId = "nonexistent-user";

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationUserNotFoundException>(async () =>
            {
                await _categoryService.GetAllFollowedCategoriesByUserIdAsync(invalidUserId);
            });
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCategory()
        {
            // Arrange
            var expectedCategory = _unitOfWorkMock.GeneratedCategories.First();
            int categoryId = expectedCategory.Id;

            // Act
            var result = await _categoryService.GetByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsCategoryNotFoundException()
        {
            // Arrange
            int invalidId = -1;

            // Act & Assert
            await Assert.ThrowsAsync<CategoryNotFoundException>(async () =>
            {
                await _categoryService.GetByIdAsync(invalidId);
            });
        }

        [Fact]
        public async Task FollowCategory_WithValidData_ReturnsTrue()
        {
            // Arrange
            var user = _unitOfWorkMock.GeneratedUsers.First();
            var category = _unitOfWorkMock.GeneratedCategories.First();

            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns(user.Id);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            if (user.FollowedCategories == null)
                user.FollowedCategories = new List<Category>();

            // Act
            var result = await _categoryService.FollowCategory(category.Id);

            // Assert
            Assert.True(result);
            Assert.Contains(user.FollowedCategories, c => c.Id == category.Id);
        }

        [Fact]
        public async Task FollowCategory_WithNullCurrentUser_ThrowsUnauthorizedException()
        {
            // Arrange
            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns((string)null!);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () =>
            {
                await _categoryService.FollowCategory(1);
            });
        }

        [Fact]
        public async Task FollowCategory_WithNonexistentUser_ThrowsApplicationUserNotFoundException()
        {
            // Arrange
            var fakeUserId = "fake-user-id";
            var category = _unitOfWorkMock.GeneratedCategories.First();

            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns(fakeUserId);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationUserNotFoundException>(async () =>
            {
                await _categoryService.FollowCategory(category.Id);
            });
        }

        [Fact]
        public async Task FollowCategory_WithNonexistentCategory_ThrowsCategoryNotFoundException()
        {
            // Arrange
            var user = _unitOfWorkMock.GeneratedUsers.First();
            var nonExistentCategoryId = 9999;

            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns(user.Id);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<CategoryNotFoundException>(async () =>
            {
                await _categoryService.FollowCategory(nonExistentCategoryId);
            });
        }

        [Fact]
        public async Task UnfollowCategory_WithValidData_ReturnsTrue()
        {
            // Arrange
            var user = _unitOfWorkMock.GeneratedUsers.First();
            var category = _unitOfWorkMock.GeneratedCategories.First();

            if (user.FollowedCategories == null)
                user.FollowedCategories = new List<Category>();
            user.FollowedCategories.Add(category);

            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns(user.Id);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            // Act
            var result = await _categoryService.UnfollowCategory(category.Id);

            // Assert
            Assert.True(result);
            Assert.DoesNotContain(user.FollowedCategories, c => c.Id == category.Id);
        }

        [Fact]
        public async Task UnfollowCategory_WithNonexistentCategory_ThrowsCategoryNotFoundException()
        {
            // Arrange
            var user = _unitOfWorkMock.GeneratedUsers.First();
            var nonExistentCategoryId = 9999;

            var mockUserContext = new Mock<ICurrentUserContext>();
            mockUserContext.Setup(c => c.UserId).Returns(user.Id);
            Mock.Get(_unitOfWorkMock.UnitOfWorkInstance)
                .Setup(u => u.CurrentUserContext)
                .Returns(mockUserContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<CategoryNotFoundException>(async () =>
            {
                await _categoryService.UnfollowCategory(nonExistentCategoryId);
            });
        }

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesCategory()
        {
            // Arrange
            var createDto = new CreateCategoryRequestDto
            {
                Name = "New Category"
            };
            var initialCount = _unitOfWorkMock.GeneratedCategories.Count;

            // Act
            var result = await _categoryService.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createDto.Name, result.Name);
            Assert.Equal(initialCount + 1, _unitOfWorkMock.GeneratedCategories.Count);
        }

        [Fact]
        public async Task CreateAsync_WithExistingCategory_ThrowsCategoryAlreadyExistsException()
        {
            // Arrange
            var existingCategory = _unitOfWorkMock.GeneratedCategories.First();
            var createDto = new CreateCategoryRequestDto
            {
                Name = existingCategory.Name
            };

            // Act & Assert
            await Assert.ThrowsAsync<CategoryAlreadyExistsException>(async () =>
            {
                await _categoryService.CreateAsync(createDto);
            });
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesCategory()
        {
            // Arrange
            var category = _unitOfWorkMock.GeneratedCategories.First();
            var updateDto = new UpdateCategoryRequestDto
            {
                Name = "Updated Category Name"
            };

            // Act
            var result = await _categoryService.UpdateAsync(category.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Id, result.Id);
            Assert.Equal(updateDto.Name, result.Name);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsCategoryNotFoundException()
        {
            // Arrange
            int invalidId = -1;
            var updateDto = new UpdateCategoryRequestDto
            {
                Name = "New Name"
            };

            // Act & Assert
            await Assert.ThrowsAsync<CategoryNotFoundException>(async () =>
            {
                await _categoryService.UpdateAsync(invalidId, updateDto);
            });
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesCategory()
        {
            // Arrange
            var category = _unitOfWorkMock.GeneratedCategories.First();
            var initialCount = _unitOfWorkMock.GeneratedCategories.Count;

            // Act
            var result = await _categoryService.DeleteAsync(category.Id);

            // Assert
            Assert.True(result);
            Assert.Equal(initialCount - 1, _unitOfWorkMock.GeneratedCategories.Count);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ThrowsCategoryNotFoundException()
        {
            // Arrange
            int invalidId = -1;

            // Act & Assert
            await Assert.ThrowsAsync<CategoryNotFoundException>(async () =>
            {
                await _categoryService.DeleteAsync(invalidId);
            });
        }
    }
}
