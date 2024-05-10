using CodeExecutor.DB.Abstractions.Models;
using CodeExecutor.DB.Abstractions.Repository;
using CodeExecutor.DB.Exceptions;
using Microsoft.EntityFrameworkCore;


namespace CodeExecutor.DB.Repository;

public class UsersRepository : DefaultEfRepository<User>, IUsersRepository
{
    public UsersRepository(DataContext context) : base(context) { }


    public async Task<User> Create(User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");

        if (string.IsNullOrWhiteSpace(user.Username))
            throw new ArgumentNullException(nameof(user.Username), "User.Username cannot be null");

        var conflicts = await Query()
            .Where(u => u.Username == user.Username)
            .CountAsync();

        if (conflicts != 0)
            throw new ConflictException(nameof(User), "username");

        dbSet.Add(user);
        return user;
    }

    public async Task<User> Update(User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");

        if (user.Id <= 0)
            throw new ArgumentNullException(nameof(user.Id), "User.Id cannot be null");

        if (string.IsNullOrWhiteSpace(user.Username))
            throw new ArgumentNullException(nameof(user.Username), "User.Username cannot be null");

        var existingUser = await GetAsync(user.Id);
        if (existingUser is null)
        {
            return await Create(user);
        }

        var conflicts = await Query()
            .Where(u => u.Id != user.Id && u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase))
            .CountAsync();

        if (conflicts != 0)
            throw new ConflictException(nameof(User), "username");

        dbSet.Update(user);
        return user;
    }

    public async Task Delete(User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");

        if (user.Id <= 0)
            throw new ArgumentNullException(nameof(user.Id), "User.Id cannot be null");

        var existingUser = await GetAsync(user.Id);
        if (existingUser != null)
            dbSet.Remove(user);
    }

    public async Task<bool> CheckExistenceAsync(string username, byte[] passwordHash)
    {
        var count = await Query()
            .Where(u => u.PasswordHash.SequenceEqual(passwordHash) &&
                        u.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
            .CountAsync();

        return count > 0;
    }
}