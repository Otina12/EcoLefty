using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Persistence.Context;

namespace EcoLefty.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly EcoLeftyDbContext _context;

    public AccountRepository(EcoLeftyDbContext context)
    {
        _context = context;
    }

    public async Task DeactivateAsync(string id, CancellationToken token = default)
    {
        var account = await _context.Accounts.FindAsync(new object?[] { id }, cancellationToken: token);

        if (account != null)
        {
            account.IsActive = false;
        }
    }
}
