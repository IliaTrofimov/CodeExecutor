using Microsoft.Extensions.Logging;


namespace CodeExecutor.UnitTests.Mocks.Repositories;

public abstract class InMemoryRepository<TEntity, TKey> : DBRepo.IReadonlyRepository<TEntity>,
                                                          DBRepo.IEditableRepository<TEntity>
    where TEntity : DBModels.BaseEntity<TKey>
    where TKey : notnull, new()
{
    protected readonly Dictionary<TKey, TEntity> Data = new();

    protected readonly ILogger? Logger;

    protected abstract TKey NextKey();

    protected InMemoryRepository(ILogger? logger = null) { Logger = logger; }


    public Task<int> SaveAsync()
    {
        Logger?.LogDebug($"MOCK {nameof(SaveAsync)}");
        return Task.FromResult(1);
    }

    public Task<TEntity?> GetAsync(object key, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug($"MOCK {nameof(GetAsync)}");

        if (key is not TKey tKey)
            throw new InvalidOperationException($"Cannot use key of type {key.GetType()}");

        Data.TryGetValue(tKey, out var entity);
        return Task.FromResult(entity);
    }

    public Task<IList<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug($"MOCK {nameof(ListAsync)}");
        return Task.FromResult<IList<TEntity>>(Data.Values.ToList());
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug($"MOCK {nameof(CountAsync)}");
        return Task.FromResult(Data.Count);
    }

    public IQueryable<TEntity> Query()
    {
        Logger?.LogDebug($"MOCK {nameof(Query)}");
        return Data.Values.AsQueryable();
    }

    public Task<TEntity> Create(TEntity item)
    {
        Logger?.LogDebug($"MOCK {nameof(Create)}");

        item.Id = NextKey();
        Data.Add(item.Id, item);
        return Task.FromResult(item);
    }

    public Task<TEntity> Update(TEntity updatedItem)
    {
        Logger?.LogDebug($"MOCK {nameof(Update)}");

        if (!Data.ContainsKey(updatedItem.Id))
            throw new KeyNotFoundException(
                $"Entity with key {updatedItem.Id}({updatedItem.Id.GetType()}) does not exist.");

        Data[updatedItem.Id] = updatedItem;
        return Task.FromResult(updatedItem);
    }

    public Task Delete(TEntity deletedItem)
    {
        Logger?.LogDebug($"MOCK {nameof(Delete)}");

        Data.Remove(deletedItem.Id);
        return Task.CompletedTask;
    }
}