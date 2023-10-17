using System.Linq.Expressions;
using AppCore.Extensions;
using AppCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AppCore.Data;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    public IQueryable<TEntity?> GetQuery();

    public IQueryable<TEntity?> GetQueryCode();

    public Task<List<TEntity?>> FindAsync(
        Expression<Func<TEntity, bool>>[]? filters,
        string orderBy,
        int skip,
        int limit);

    public Task<List<TEntity?>> FindAsync(
        Expression<Func<TEntity, bool>>[]? filters,
        string? orderBy);
    
    public Task<List<TDto>> FindAsync<TDto>(
        Expression<Func<TEntity, bool>>[]? filters,
        string orderBy);
    
    public Task<List<TDto>> FindAsync<TDto>(
        Expression<Func<TEntity, bool>>[]? filters,
        string orderBy,
        int skip,
        int limit);

    public Task<TDto?> FindOneAsync<TDto>(Expression<Func<TEntity, bool>>[]? filters,
        string? orderBy = null);

    public Task<int> CountAsync(
        Expression<Func<TEntity, bool>>[]? filters);

    public Task<bool> IsAlreadyExistAsync(
        Expression<Func<TEntity, bool>>[]? filters);

    public Task<bool> IsAlreadyExistAsync(Guid systemId);
    public Task<bool> IsAlreadyExistAsync(IEnumerable<Guid> systemIds);

    public Task<FindResult<TEntity>> FindResultAsync(
        Expression<Func<TEntity, bool>>[]? filters,
        string orderBy,
        int skip,
        int limit);
    
    public Task<FindResult<TDto>> FindResultAsync<TDto>(
        Expression<Func<TEntity, bool>>[]? filters,
        string orderBy,
        int skip,
        int limit);

    public Task<TEntity?> FindOneAsync(Guid systemId);

    public Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>>[]? filters,
        string orderBy = null!);
    
    public Task<bool> InsertAsync(TEntity? entity, Guid? creatorId, DateTime? now = null);
    public Task<bool> InsertAsync(IEnumerable<TEntity?> entities, Guid? creatorId, DateTime? now = null);
    public Task<bool> UpdateAsync(TEntity? entity, Guid? editorId, DateTime? now = null);
    public Task<bool> UpdateAsync(IEnumerable<TEntity?> entities, Guid? editorId, DateTime? now = null);
    public Task<bool> DeleteAsync(TEntity? entity, Guid? deleterId, DateTime? now = null);
    public Task<bool> DeleteAsync(IEnumerable<TEntity?> entities, Guid? deleterId, DateTime? now = null);
}

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly IUnitOfWork _unitOfWork;

    public BaseRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<TEntity>();
        _unitOfWork = new UnitOfWork(dbContext);
    }

    public IQueryable<TEntity?> GetQuery() => _dbSet.Where(x => !x.DeletedAt.HasValue);

    public IQueryable<TEntity?> GetQueryCode() => _dbSet;

    public async Task<List<TEntity?>> FindAsync(Expression<Func<TEntity, bool>>[]? filters, string orderBy, int skip,
        int limit)
    {
        IQueryable<TEntity?> query = _dbSet;
        query = query.Where(x => x != null && !x.DeletedAt.HasValue);
        if (filters != null && filters.Any())
            query = filters.Aggregate(query, (current, filter) => current.Where(filter!));

        if (!string.IsNullOrEmpty(orderBy))
            query = OrderBy(query, orderBy);

        if (skip > 0)
            query = query.Skip(skip);

        if (limit > 0)
            query = query.Take(limit);

        return await query.ToListAsync();
    }

    public async Task<List<TEntity?>> FindAsync(Expression<Func<TEntity, bool>>[]? filters, string? orderBy)
    {
        IQueryable<TEntity?> query = _dbSet;
        query = query.Where(x => x != null && !x.DeletedAt.HasValue);
        if (filters != null && filters.Any())
            query = filters.Aggregate(query, (current, filter) => current.Where(filter!));

        if (!string.IsNullOrEmpty(orderBy))
            query = OrderBy(query, orderBy);

        return await query.ToListAsync();
    }
    
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>[]? filters)
    {
        IQueryable<TEntity?> query = _dbSet;
        query = query.Where(x => x != null && !x.DeletedAt.HasValue);
        if (filters != null && filters.Any())
            query = filters.Aggregate(query, (current, filter) => current.Where(filter!));
        return await query.CountAsync();
    }

    public async Task<bool> IsAlreadyExistAsync(Expression<Func<TEntity, bool>>[]? filters)
    {
        IQueryable<TEntity?> query = _dbSet;
        query = query.Where(x => x != null && !x.DeletedAt.HasValue);
        if (filters != null && filters.Any())
            query = filters.Aggregate(query, (current, filter) => current.Where(filter!));
        return await query.AnyAsync();
    }

    public async Task<bool> IsAlreadyExistAsync(Guid systemId)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == systemId && !x.DeletedAt.HasValue) != null;
    }

    public async Task<bool> IsAlreadyExistAsync(IEnumerable<Guid> systemIds)
    {
        return await _dbSet.CountAsync(x => !x.DeletedAt.HasValue && systemIds.Contains(x.Id)) == systemIds.Count();
    }

    public async Task<FindResult<TEntity>> FindResultAsync(Expression<Func<TEntity, bool>>[]? filters, string orderBy,
        int skip, int limit)
    {
        IQueryable<TEntity?> query = _dbSet;
        query = query.Where(x => x != null && !x.DeletedAt.HasValue);
        if (filters != null && filters.Any())
            query = filters.Aggregate(query, (current, filter) => current.Where(filter!));

        if (!string.IsNullOrEmpty(orderBy))
            query = OrderBy(query, orderBy);

        var totalCount = await query.LongCountAsync();
        if (skip > 0)
            query = query.Skip(skip);

        if (limit > 0)
            query = query.Take(limit);

        var items = await query.ToListAsync();
        return FindResult<TEntity>.Success(items!, totalCount);
    }
    
    public async Task<TEntity?> FindOneAsync(Guid systemId)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == systemId && !x.DeletedAt.HasValue);
    }
    

    public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>>[]? filters, string orderBy = null!)
    {
        IQueryable<TEntity?> query = _dbSet;
        query = query.Where(x => !x!.DeletedAt.HasValue);
        if (filters != null && filters.Any())
            query = filters.Aggregate(query, (current, filter) => current.Where(filter!));

        if (!string.IsNullOrEmpty(orderBy))
            query = OrderBy(query, orderBy);

        return await query.FirstOrDefaultAsync();
    }
    
    public async Task<bool> InsertAsync(TEntity? entity, Guid? creatorId, DateTime? now = null)
    {
        await _unitOfWork.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        entity!.CreatedAt = now.Value;
        entity.EditedAt = now.Value;
        entity.CreatorId = creatorId;
        await _dbSet.AddAsync(entity);
        await _unitOfWork.SaveAsync();
        return await _unitOfWork.CommitTransactionAsync();
    }

    public async Task<bool> InsertAsync(IEnumerable<TEntity?> entities, Guid? creatorId, DateTime? now = null)
    {
        await _unitOfWork.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        entities = entities.Select(x =>
        {
            x!.CreatedAt = now.Value;
            x.EditedAt = now.Value;
            x.CreatorId = creatorId;
            return x;
        });
        await _dbSet.AddRangeAsync(entities!);
        await _unitOfWork.SaveAsync();
        return await _unitOfWork.CommitTransactionAsync();
    }

    public async Task<bool> UpdateAsync(TEntity? entity, Guid? editorId, DateTime? now = null)
    {
        await _unitOfWork.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        entity!.EditedAt = now.Value;
        _dbSet.Entry(entity).State = EntityState.Modified;
        await _unitOfWork.SaveAsync();
        return await _unitOfWork.CommitTransactionAsync();
    }
    public async Task<bool> UpdateAsyncValid(TEntity? entity, Guid? editorId, DateTime? now = null)
    {
        await _unitOfWork.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        entity!.EditedAt = now.Value;
        _dbSet.Entry(entity).State = EntityState.Modified;

        try
        {
            await _unitOfWork.SaveAsync();
            return await _unitOfWork.CommitTransactionAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Handle the concurrency conflict here, e.g., log the conflict or notify the user.
            // You can also retry the update if desired.
            // For example, you can fetch the current state of the entity from the database and merge changes.
            return false; // Indicate that the update failed due to concurrency conflict.
        }
    }

    public async Task<bool> UpdateAsync(IEnumerable<TEntity?> entities, Guid? editorId, DateTime? now = null)
    {
        await _unitOfWork.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        entities = entities.Select(x =>
        {
            x!.EditedAt = now.Value;
            return x;
        });
        foreach (var entity in entities)
        {
            _dbSet.Entry(entity!).State = EntityState.Modified;
        }

        await _unitOfWork.SaveAsync();
        return await _unitOfWork.CommitTransactionAsync();
    }

    public async Task<bool> DeleteAsync(TEntity? entity, Guid? deleterId, DateTime? now = null)
    {
        await _unitOfWork.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        entity!.DeletedAt = now.Value;
        _dbSet.Entry(entity).State = EntityState.Modified;
        await _unitOfWork.SaveAsync();
        return await _unitOfWork.CommitTransactionAsync();
    }

    public async Task<bool> DeleteAsync(IEnumerable<TEntity?> entities, Guid? deleterId, DateTime? now = null)
    {
        await _unitOfWork.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        entities = entities.Select(x =>
        {
            x!.DeletedAt = now.Value;
            return x;
        });
        foreach (var entity in entities)
        {
            _dbSet.Entry(entity!).State = EntityState.Modified;
        }

        await _unitOfWork.SaveAsync();
        return await _unitOfWork.CommitTransactionAsync();
    }

    private static IQueryable<TEntity> OrderBy(IQueryable<TEntity?> query, string orderBy)
    {
        var propertyName = orderBy.Split(" ")[0];
        query = orderBy.Contains("desc")
            ? query.OrderByDescending(x => EF.Property<TEntity>(x!, propertyName))
            : query.OrderBy(x => EF.Property<TEntity>(x!, propertyName));
        return query!;
    }
    
    public async Task<List<TDto>> FindAsync<TDto>(Expression<Func<TEntity, bool>>[]? filters, string? orderBy)
    {
        IQueryable<TEntity> query = _dbSet;
        query = query.Where(x => !x.DeletedAt.HasValue);
        if (filters != null && filters.Any())
            query = filters.Aggregate(query, (current, filter) => current.Where(filter));

        if (!string.IsNullOrEmpty(orderBy))
            query = OrderBy(query, orderBy);

        var result = await query.ToListAsync();
        return result.ProjectTo<TEntity, TDto>();
    }
    
    public async Task<List<TDto>> FindAsync<TDto>(Expression<Func<TEntity, bool>>[]? filters, string orderBy, int skip,
        int limit)
    {
        IQueryable<TEntity> query = _dbSet;
        query = query.Where(x => !x.DeletedAt.HasValue);
        if (filters != null && filters.Any())
            query = filters.Aggregate(query, (current, filter) => current.Where(filter));

        if (!string.IsNullOrEmpty(orderBy))
            query = OrderBy(query, orderBy);

        if (skip > 0)
            query = query.Skip(skip);

        if (limit > 0)
            query = query.Take(limit);

        var result = await query.ToListAsync();
        return result.ProjectTo<TEntity, TDto>();
    }
    
    public async Task<FindResult<TDto>> FindResultAsync<TDto>(Expression<Func<TEntity, bool>>[]? filters, string orderBy,
        int skip, int limit)
    {
        IQueryable<TEntity> query = _dbSet;
        query = query.Where(x => !x.DeletedAt.HasValue);
        if (filters != null && filters.Any())
            query = filters.Aggregate(query, (current, filter) => current.Where(filter));

        if (!string.IsNullOrEmpty(orderBy))
            query = OrderBy(query, orderBy);

        var totalCount = await query.LongCountAsync();
        if (skip > 0)
            query = query.Skip(skip);

        if (limit > 0)
            query = query.Take(limit);

        var result = await query.ToListAsync();
        return FindResult<TDto>.Success(result.ProjectTo<TEntity, TDto>(), totalCount);
    }
    
    public async Task<TDto?> FindOneAsync<TDto>(Expression<Func<TEntity, bool>>[]? filters, string? orderBy = null)
    {
        IQueryable<TEntity> query = _dbSet;
        query = query.Where(x => !x.DeletedAt.HasValue);
        if (filters != null && filters.Any())
            query = filters.Aggregate(query, (current, filter) => current.Where(filter));

        if (!string.IsNullOrEmpty(orderBy))
            query = OrderBy(query, orderBy);

        var result = await query.FirstOrDefaultAsync();
        return result!.ProjectTo<TEntity, TDto>();
    }

}