using AutoMapper;
using EcoLefty.Application.Products.DTOs;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Common.IncludeExpressions;
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

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsOfCompanyAsync(string id, CancellationToken token = default)
    {
        var products = await _unitOfWork.Products.GetAllWhereAsync(x => x.CompanyId == id, trackChanges: false, token: token);
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<ProductDetailsResponseDto> GetByIdAsync(int id, CancellationToken token = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, trackChanges: false, token: token,
            ProductIncludes.Company,
            ProductIncludes.Categories,
            ProductIncludes.Offers);

        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }

        return _mapper.Map<ProductDetailsResponseDto>(product);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductRequestDto createProductDto, CancellationToken token = default)
    {
        var currentCompanyId = _unitOfWork.CurrentUserContext.UserId;

        if (currentCompanyId is null)
            throw new UnauthorizedException();

        var existingProduct = await _unitOfWork.Products.GetOneWhereAsync(
        x => x.Name == createProductDto.Name && x.CompanyId == currentCompanyId,
        trackChanges: false,
        token: token);

        if (existingProduct is not null)
            throw new ProductAlreadyExistsException(createProductDto.Name);

        var product = _mapper.Map<Product>(createProductDto);
        var company = await _unitOfWork.Companies.GetByIdAsync(currentCompanyId, false, token);

        if (company is null)
            throw new CompanyNotFoundException($"Company attached to account with Id: {currentCompanyId} was not found");

        if (!company.IsApproved)
            throw new CompanyNotApprovedException(company.Id);

        product.CompanyId = company.Id;

        if (createProductDto.CategoryIds != null && createProductDto.CategoryIds.Any())
        {
            product.Categories = new List<Category>();
            foreach (var categoryId in createProductDto.CategoryIds)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, true, token);
                if (category is null)
                {
                    throw new CategoryNotFoundException(categoryId);
                }
                product.Categories.Add(category);
            }
        }

        await _unitOfWork.Products.CreateAsync(product, token);
        await _unitOfWork.SaveChangesAsync(token);

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto> UpdateAsync(int id, UpdateProductRequestDto updateProductDto, CancellationToken token = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, true, token);
        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }

        _mapper.Map(updateProductDto, product);

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(token);

        return _mapper.Map<ProductResponseDto>(product);
    }

    /// <summary>
    /// Soft deletes an entity and all related entities.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ProductNotFoundException"></exception>
    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, true, token);
        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }

        _unitOfWork.Products.Delete(product);

        var deleted = await _unitOfWork.SaveChangesAsync(token);
        return deleted > 0;
    }
}
