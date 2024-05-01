using CodeExecutor.DB.Models;

namespace CodeExecutor.DB.Repository;

public interface IUsersRepository : IReadonlyRepository<User>, IEditableRepository<User>
{
    /// <summary>
    /// Check if user with given username and password exists.
    /// </summary>
    public Task<bool> CheckExistenceAsync(string username, byte[] passwordHash);
}