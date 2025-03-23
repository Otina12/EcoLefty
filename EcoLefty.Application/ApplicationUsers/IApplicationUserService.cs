using EcoLefty.Application.ApplicationUsers.DTOs;

namespace EcoLefty.Application.ApplicationUsers;

public interface IApplicationUserService
{
    Task<IEnumerable<ApplicationUserResponseDto>> GetAllAsync(CancellationToken token = default);
    Task<ApplicationUserDetailsResponseDto> GetByIdAsync(int id, CancellationToken token = default);
    Task<ApplicationUserResponseDto> CreateAsync(CreateApplicationUserRequestDto createUserDto, CancellationToken token = default);
    Task<ApplicationUserResponseDto> UpdateAsync(int id, UpdateApplicationUserRequestDto updateUserDto, CancellationToken token = default);
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}