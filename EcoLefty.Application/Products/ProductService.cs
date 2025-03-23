using AutoMapper;
using EcoLefty.Application.Products.DTOs;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application.Products;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(CancellationToken token = default)
    {
        var products = await _unitOfWork.Products.GetAllAsync(trackChanges: false, token: token);
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<ProductDetailsResponseDto> GetByIdAsync(int id, CancellationToken token = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }

        return _mapper.Map<ProductDetailsResponseDto>(product);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductRequestDto createProductDto, CancellationToken token = default)
    {
        var existingProduct = await _unitOfWork.Products.GetOneWhereAsync(
            x => x.Name == createProductDto.Name,
            trackChanges: false,
            token: token);

        if (existingProduct is not null)
        {
            throw new ProductAlreadyExistsException(createProductDto.Name);
        }

        // ---------------- TODO: assign urrent company ID ----------------

        var product = _mapper.Map<Product>(createProductDto);

        await _unitOfWork.Products.CreateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto> UpdateAsync(int id, UpdateProductRequestDto updateProductDto, CancellationToken token = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }

        _mapper.Map(updateProductDto, product);

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }

        _unitOfWork.Products.Delete(product);

        var deleted = await _unitOfWork.SaveChangesAsync();
        return deleted > 0;
    }
}
