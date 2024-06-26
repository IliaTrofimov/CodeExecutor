using CodeExecutor.DB.Abstractions.Models;
using CodeExecutor.DB.Abstractions.Repository;
using Microsoft.EntityFrameworkCore;


namespace CodeExecutor.DB.Repository;

public sealed class LanguagesRepository : DefaultEfRepository<Language>, ILanguagesRepository
{
    public LanguagesRepository(DataContext context) : base(context) { }


    public async Task<List<Language>>
        ListVersionsAsync(string languageName, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(l => l.Name == languageName)
            .OrderBy(l => l.Version)
            .ToListAsync(cancellationToken);
    }
}