using CodeExecutor.Dispatcher.Contracts;

namespace CodeExecutor.Dispatcher.Host.Services.Interfaces;

/// <summary>
/// Programming languages manage.
/// </summary>
public interface IProgrammingLanguagesService
{
    /// <summary>Get all language objects.</summary>
    public Task<List<Language>> GetListAsync(int? skip = null, int? take = null);
    
    /// <summary>Get all language objects.</summary>
    public Task<List<Language>> GetVersionsListAsync(string languageName);
}