namespace EcoLefty.Domain.Contracts;

public interface ITransactionWrapper
{
    Task BeginTransactionAsync(CancellationToken token = default);
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
