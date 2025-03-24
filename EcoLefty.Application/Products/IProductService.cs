using EcoLefty.Application.Products.DTOs;

namespace EcoLefty.Application.Products;

public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllAsync(CancellationToken token = default);
    Task<ProductDetailsResponseDto> GetByIdAsync(int id, CancellationToken token = default);
    Task<ProductResponseDto> CreateAsync(CreateProductRequestDto createProductDto, CancellationToken token = default);
    Task<ProductResponseDto> UpdateAsync(int id, UpdateProductRequestDto updateProductDto, CancellationToken token = default);
    /// <summary>
    /// Soft deletes an entity and all related entities.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ProductNotFoundException"></exception>
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}