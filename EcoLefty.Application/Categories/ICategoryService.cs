using EcoLefty.Application.Categories.DTOs;

namespace EcoLefty.Application.Categories;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponseDto>> GetAllAsync(CancellationToken token = default);
    Task<CategoryResponseDto> GetByIdAsync(int id, CancellationToken token = default);
    Task<CategoryResponseDto> CreateAsync(CreateCategoryRequestDto dto, CancellationToken token = default);
    Task<CategoryResponseDto> UpdateAsync(int id, UpdateCategoryRequestDto dto, CancellationToken token = default);
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}