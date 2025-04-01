using EcoLefty.Application.Categories.DTOs;

namespace EcoLefty.Application.Categories;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponseDto>> GetAllAsync(CancellationToken token = default);
    Task<IEnumerable<CategoryResponseDto>> GetAllFollowedCategoriesByUserIdAsync(string userId, CancellationToken token = default);
    Task<CategoryResponseDto> GetByIdAsync(int id, CancellationToken token = default);
    Task<bool> FollowCategory(int categoryId, CancellationToken token = default);
    Task<bool> UnfollowCategory(int categoryId, CancellationToken token = default);
    Task<CategoryResponseDto> CreateAsync(CreateCategoryRequestDto dto, CancellationToken token = default);
    Task<CategoryResponseDto> UpdateAsync(int id, UpdateCategoryRequestDto dto, CancellationToken token = default);
    /// <summary>
    /// Soft deletes an entity and all related entities.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="CategoryNotFoundException"></exception>
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}