using Microsoft.EntityFrameworkCore;

namespace CodeExecutor.DB.Repository;

public static class RepositoryExtensions
{
    public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
    {
        return source.ToListAsync(default);
    }
}