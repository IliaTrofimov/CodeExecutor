using CodeExecutor.DB.Abstractions.Models;


namespace CodeExecutor.DB.Abstractions.Repository;

public interface IUsersRepository : IReadonlyRepository<User>, IEditableRepository<User>
{
    /// <summary>Check if user with given username and password exists.</summary>
    public Task<bool> CheckExistenceAsync(string username, byte[] passwordHash);
}