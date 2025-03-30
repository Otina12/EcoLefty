using EcoLefty.Domain.Contracts.Repositories.Common;
using EcoLefty.Domain.Entities.Identity;

namespace EcoLefty.Domain.Contracts.Repositories;

public interface IAccountRepository : IBaseRepository<Account, string>
{
    Task DeactivateAsync(string id, CancellationToken token = default);
}