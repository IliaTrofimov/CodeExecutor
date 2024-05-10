using Microsoft.Extensions.Logging;


namespace CodeExecutor.UnitTests.Mocks.Repositories;

public class LanguagesRepositoryMock : InMemoryRepository<DBModels.Language, long>, DBRepo.ILanguagesRepository
{
    public const long Missing = -1;
    public const long CSharpId = 1;
    public const long PythonId = 2;
    public const long PascalId = 3;

    public LanguagesRepositoryMock(ILogger? logger = null) : base(logger)
    {
        Data.Add(1, new DBModels.Language { Name = "CSharp", Version = "12", Id = CSharpId });
        Data.Add(2, new DBModels.Language { Name = "Python", Version = "10", Id = PythonId });
        Data.Add(3, new DBModels.Language { Name = "PascalABC.Net", Id = PascalId });
    }


    public Task<List<DBModels.Language>> ListVersionsAsync(string languageName,
                                                           CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug($"MOCK {nameof(ListVersionsAsync)}");
        return Task.FromResult(Data.Values.Where(l => l.Name == languageName).ToList());
    }

    protected override long NextKey() => Data.Count;
}