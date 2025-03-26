using AutoMapper;
using EcoLefty.Application.Categories.DTOs;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application.Categories;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync(CancellationToken token = default)
    {
        var categories = await _unitOfWork.Categories.GetAllAsync(trackChanges: false, token: token);
        return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
    }

    public async Task<CategoryResponseDto> GetByIdAsync(int id, CancellationToken token = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, false, token);
        if (category is null)
        {
            throw new CategoryNotFoundException(id);
        }

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryRequestDto createCategoryDto, CancellationToken token = default)
    {
        var existingCategory = await _unitOfWork.Categories.GetOneWhereAsync(x => x.Name == createCategoryDto.Name, trackChanges: false, token: token);
        if (existingCategory is not null)
        {
            throw new CategoryAlreadyExistsException(createCategoryDto.Name);
        }

        var category = _mapper.Map<Category>(createCategoryDto);

        await _unitOfWork.Categories.CreateAsync(category, token);
        await _unitOfWork.SaveChangesAsync(token);

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<CategoryResponseDto> UpdateAsync(int id, UpdateCategoryRequestDto updateCategoryDto, CancellationToken token = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, true, token);
        if (category is null)
        {
            throw new CategoryNotFoundException(id);
        }

        _mapper.Map(updateCategoryDto, category);

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync(token);

        return _mapper.Map<CategoryResponseDto>(category);
    }

    /// <summary>
    /// Soft deletes an entity and all related entities.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="CategoryNotFoundException"></exception>
    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, false, token);
        if (category is null)
        {
            throw new CategoryNotFoundException(id);
        }

        _unitOfWork.Categories.Delete(category);

        var deleted = await _unitOfWork.SaveChangesAsync(token);
        return deleted > 0;
    }
}
