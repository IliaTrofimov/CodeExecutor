using System.Collections;

namespace CodeExecutor.DB.Repository;


public interface IRepository
{
    public Task<int> SaveAsync();
}