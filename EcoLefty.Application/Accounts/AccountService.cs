using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities.Identity;

namespace EcoLefty.Application.Accounts;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Account> GetAccountByIdAsync(string id, CancellationToken token = default)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(id, false, token);
        if (account is null)
        {
            throw new AccountNotFoundException();
        }

        return account;
    }
}
