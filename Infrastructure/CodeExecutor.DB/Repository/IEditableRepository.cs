namespace CodeExecutor.DB.Repository;

public interface IEditableRepository<TItem> : IRepository
    where TItem: class
{
    /// <summary>Create new item and return it.</summary>
    public Task<TItem> Create(TItem item);
    
    /// <summary>Update item and return it.</summary>
    public Task<TItem> Update(TItem updatedItem);
    
    /// <summary>Delete item.</summary>
    public Task Delete(TItem deletedItem);
}