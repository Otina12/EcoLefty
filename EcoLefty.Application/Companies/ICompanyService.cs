using EcoLefty.Application.Companies.DTOs;

namespace EcoLefty.Application.Companies;

public interface ICompanyService
{
    Task<IEnumerable<CompanyResponseDto>> GetAllAsync(CancellationToken token = default);
    Task<CompanyDetailsResponseDto> GetByIdAsync(int id, CancellationToken token);
    Task<CompanyResponseDto> CreateAsync(CreateCompanyRequestDto createCompanyDto, CancellationToken token = default);
    Task<CompanyResponseDto> UpdateAsync(int id, UpdateCompanyRequestDto updateCompanyDto, CancellationToken token = default);
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}