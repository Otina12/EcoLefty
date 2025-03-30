using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities.Identity;
using EcoLefty.Infrastructure.Repositories.Common;
using EcoLefty.Persistence.Context;

namespace EcoLefty.Infrastructure.Repositories;

public class AccountRepository : BaseRepository<Account, string>, IAccountRepository
{
    public AccountRepository(EcoLeftyDbContext context) : base(context)
    {
    }

    public async Task DeactivateAsync(string id, CancellationToken token = default)
    {
        var account = await dbSet.FindAsync(new object?[] { id }, cancellationToken: token);

        if (account != null)
        {
            account.IsActive = false;
        }
    }
}
