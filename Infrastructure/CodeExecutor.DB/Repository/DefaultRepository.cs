using Microsoft.EntityFrameworkCore;

namespace CodeExecutor.DB.Repository;

/// <summary>
/// Default Entity Framework repository.
/// </summary>
public abstract class DefaultEfRepository<TItem> : IReadonlyRepository<TItem>
    where TItem : class
{
    protected readonly DbContext context;
    protected readonly DbSet<TItem> dbSet;

    protected DefaultEfRepository(DbContext context)
    {
        this.context = context;
        this.dbSet = this.context.Set<TItem>();
    }
    
    protected DefaultEfRepository(DbContext context, DbSet<TItem> dbSet)
    {
        this.context = context;
        this.dbSet = dbSet;
    }
    
    
    public async Task<int> SaveAsync()
    {
        var count = await context.SaveChangesAsync();
        return count;
    }

    public virtual async Task<TItem?> GetAsync(object key, CancellationToken cancellationToken = default)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key), "Item key cannot be null");

        if (cancellationToken.IsCancellationRequested)
            return null;
        
        var item = await dbSet.FindAsync(key, cancellationToken);
        return item;
    }

    public virtual async Task<IList<TItem>> ListAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return new List<TItem>(0);
        
        var items = await dbSet.ToListAsync(cancellationToken);
        return items;
    }
    
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return 0;
        
        var count = await dbSet.CountAsync(cancellationToken);
        return count;
    }

    public virtual IQueryable<TItem> Query()
    {
        return dbSet.AsQueryable();
    }
}