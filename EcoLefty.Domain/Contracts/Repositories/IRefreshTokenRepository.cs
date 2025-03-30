using EcoLefty.Domain.Entities.Auth;

namespace EcoLefty.Domain.Contracts.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetValidTokenAsync(string accountId, string token);
    Task AddAsync(RefreshToken refreshToken);
    void Remove(RefreshToken refreshToken);
    Task SaveAsync();
}
