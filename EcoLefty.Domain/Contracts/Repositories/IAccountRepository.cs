namespace EcoLefty.Domain.Contracts.Repositories;

public interface IAccountRepository
{
    Task DeactivateAsync(string id, CancellationToken token = default);
}