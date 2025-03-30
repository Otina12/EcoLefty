using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Application.Companies.DTOs;

namespace EcoLefty.Application.Companies;

public interface ICompanyService
{
    Task<IEnumerable<CompanyResponseDto>> GetAllAsync(CancellationToken token = default);
    Task<CompanyDetailsResponseDto> GetByIdAsync(int id, CancellationToken token);
    Task<CompanyDetailsResponseDto> GetByAccountIdAsync(string accountId, CancellationToken token);
    /// <summary>
    /// Creates a company entity and a corresponding account. Same as registration.
    /// </summary>
    /// <param name="createCompanyDto"></param>
    /// <param name="token"></param>
    /// <returns>A JWT token</returns>
    /// <exception cref="ApplicationUserNotFoundException"></exception>
    Task<TokenResponseDto> CreateAsync(CreateCompanyRequestDto createCompanyDto, CancellationToken token = default);
    Task<CompanyResponseDto> UpdateAsync(int id, UpdateCompanyRequestDto updateCompanyDto, CancellationToken token = default);
    Task<bool> ApproveCompanyAsync(int id, CancellationToken token = default);
    /// <summary>
    /// Soft deletes an entity and all related entities.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="CompanyNotFoundException"></exception>
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}