using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities.Auth;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace EcoLefty.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly EcoLeftyDbContext _context;

    public RefreshTokenRepository(EcoLeftyDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetValidTokenAsync(string accountId, string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.AccountId == accountId && r.Token == token && r.ExpiresOnUtc > DateTime.UtcNow);
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
    }

    public void Remove(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Remove(refreshToken);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
