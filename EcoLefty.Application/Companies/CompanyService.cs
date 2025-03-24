using AutoMapper;
using EcoLefty.Application.Accounts;
using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.IncludeExpressions;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application.Companies;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountService _authService;
    private readonly IMapper _mapper;

    public CompanyService(IUnitOfWork unitOfWork, IMapper mapper, IAccountService authService)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CompanyResponseDto>> GetAllAsync(CancellationToken token = default)
    {
        var companies = await _unitOfWork.Companies.GetAllAsync(trackChanges: false, token: token, CompanyIncludes.Account);
        return _mapper.Map<IEnumerable<CompanyResponseDto>>(companies);
    }

    public async Task<CompanyDetailsResponseDto> GetByIdAsync(int id, CancellationToken token)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id, trackChanges: false, token: token,
            CompanyIncludes.Account,
            CompanyIncludes.Products
            );

        if (company is null)
        {
            throw new CompanyNotFoundException(id);
        }

        return _mapper.Map<CompanyDetailsResponseDto>(company);
    }

    public async Task<CompanyResponseDto> CreateAsync(CreateCompanyRequestDto createCompanyDto, CancellationToken token = default)
    {
        // We need a transaction to ensure that identity user is not added without company and vice versa
        using var transaction = await _unitOfWork.BeginTransactionAsync(token);

        try
        {
            // First, we need to register identity user
            var accountDto = _mapper.Map<RegisterAccountRequestDto>(createCompanyDto);
            string accountId = await _authService.RegisterAccountAsync(accountDto, AccountRole.Company);

            // Then we can create the company
            var company = _mapper.Map<Company>(createCompanyDto);
            company.AccountId = accountId;

            await _unitOfWork.Companies.CreateAsync(company);
            await _unitOfWork.SaveChangesAsync(token);

            await transaction.CommitAsync(token);

            return _mapper.Map<CompanyResponseDto>(company);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(); // without token
            throw;
        }
    }

    public async Task<CompanyResponseDto> UpdateAsync(int id, UpdateCompanyRequestDto updateCompanyDto, CancellationToken token = default)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id, trackChanges: true, token: token);
        if (company is null)
        {
            throw new CompanyNotFoundException(id);
        }

        _mapper.Map(updateCompanyDto, company);

        _unitOfWork.Companies.Update(company);
        await _unitOfWork.SaveChangesAsync(token);

        return _mapper.Map<CompanyResponseDto>(company);
    }

    /// <summary>
    /// Soft deletes an entity and all related entities.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="CompanyNotFoundException"></exception>
    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id, trackChanges: false, token: token);
        if (company is null)
        {
            throw new CompanyNotFoundException(id);
        }

        _unitOfWork.Companies.Delete(company);

        // Because of soft delete, we need deactivate related account manually.
        // Other entities will be soft deleted using custom cascading soft delete when saving changes.
        await _unitOfWork.Accounts.DeactivateAsync(company.AccountId, token);

        var deleted = await _unitOfWork.SaveChangesAsync(token);
        return deleted > 0;
    }
}
