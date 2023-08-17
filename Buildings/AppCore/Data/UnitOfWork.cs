using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppCore.Data;

public interface IUnitOfWork : IDisposable
{
    Task<bool> SaveAsync();
    Task BeginTransactionAsync();
    Task<bool> CommitTransactionAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private IDbContextTransaction _transaction;

    public UnitOfWork(DbContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task<bool> CommitTransactionAsync()
    {
        try
        {
            var result = await _context.SaveChangesAsync() >= 0;
            await _transaction.CommitAsync();
            return result;
        }
        catch
        {
            await _transaction.RollbackAsync();
            return false;
        }
        finally
        {
            _transaction.Dispose();
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
        _transaction?.Dispose();
    }
}