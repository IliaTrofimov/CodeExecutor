namespace CodeExecutor.DB.Abstractions.Repository;


public interface IRepository
{
    public Task<int> SaveAsync();
}