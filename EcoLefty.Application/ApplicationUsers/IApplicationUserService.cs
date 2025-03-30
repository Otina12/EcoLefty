using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.Authentication.Tokens.DTOs;

namespace EcoLefty.Application.ApplicationUsers;

public interface IApplicationUserService
{
    Task<IEnumerable<ApplicationUserResponseDto>> GetAllAsync(CancellationToken token = default);
    Task<ApplicationUserDetailsResponseDto> GetByIdAsync(int id, CancellationToken token = default);
    Task<ApplicationUserDetailsResponseDto> GetByAccountIdAsync(string accountId, CancellationToken token);

    /// <summary>
    /// Creates a user entity and a corresponding account. Same as registration.
    /// </summary>
    /// <param name="createUserDto"></param>
    /// <param name="token"></param>
    /// <returns>A JWT token</returns>
    /// <exception cref="ApplicationUserNotFoundException"></exception>
    Task<TokenResponseDto> CreateAsync(CreateApplicationUserRequestDto createUserDto, CancellationToken token = default);
    Task<ApplicationUserResponseDto> UpdateAsync(int id, UpdateApplicationUserRequestDto updateUserDto, CancellationToken token = default);
    /// <summary>
    /// Soft deletes an entity and all related entities.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationUserNotFoundException"></exception>
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}