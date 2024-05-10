using CodeExecutor.DB.Abstractions.Models;


namespace CodeExecutor.DB.Abstractions.Repository;

public interface ILanguagesRepository : IReadonlyRepository<Language>
{
    /// <summary>Get all versions for given language.</summary>
    public Task<List<Language>> ListVersionsAsync(string languageName, CancellationToken cancellationToken = default);
}