namespace CodeExecutor.DB.Abstractions.Repository;

/// <summary>Basic repository with read operations.</summary>
/// <typeparam name="TItem">Type of entity.</typeparam>
public interface IReadonlyRepository<TItem> : IRepository
    where TItem : class
{
    /// <summary>Get item with given key.</summary>
    /// <returns>Item with given key or null if nothing was found.</returns>
    public Task<TItem?> GetAsync(object key, CancellationToken cancellationToken = default);

    /// <summary>List all items.</summary>
    /// <returns>List of all items.</returns>
    public Task<IList<TItem>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Count all items.</summary>
    /// <returns>Amount of all items.</returns>
    public Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>Start query operation.</summary>
    /// <returns>Query object.</returns>
    public IQueryable<TItem> Query();
}