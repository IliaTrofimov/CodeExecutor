using Microsoft.Extensions.Logging;


namespace CodeExecutor.UnitTests.Mocks.Repositories;

public class UsersRepositoryMock : InMemoryRepository<DBModels.User, long>, DBRepo.IUsersRepository
{
    public UsersRepositoryMock(ILogger? logger = null) : base(logger) {}
    
    public Task<bool> CheckExistenceAsync(string username, byte[] passwordHash)
    {
        Logger?.LogDebug($"MOCK {nameof(CheckExistenceAsync)}");

        var count = Data.Values.Count(u => u.Username == username && CompareHashes(u.PasswordHash, passwordHash));
        return Task.FromResult(count == 1);
    }

    private static bool CompareHashes(byte[] expected, byte[] actual)
    {
        if (expected.Length != actual.Length)
            return false;
        
        for (var i = 0; i < expected.Length; i++) 
            if (expected[i] != actual[i]) return false;
        return true;
    }
    
    
    protected override long NextKey() => Data.Count;
}