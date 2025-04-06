using EcoLefty.Domain.Contracts;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace EcoLefty.Infrastructure.Repositories.Common;

public class TransactionWrapper : ITransactionWrapper
{
    private readonly EcoLeftyDbContext _context;
    private IDbContextTransaction? _transaction;

    public TransactionWrapper(EcoLeftyDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync(CancellationToken token = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(token);
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is currently in progress.");
        }

        await _transaction.CommitAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is currently in progress.");
        }

        await _transaction.RollbackAsync();
    }
}
