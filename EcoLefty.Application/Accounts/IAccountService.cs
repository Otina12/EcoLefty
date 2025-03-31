using EcoLefty.Domain.Entities.Identity;

namespace EcoLefty.Application.Accounts;

public interface IAccountService
{
    Task<Account> GetAccountByIdAsync(string id, CancellationToken token = default);
}