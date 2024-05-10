using AutoMapper;
using CodeExecutor.DB.Repository;
using CodeExecutor.Dispatcher.Services.Interfaces;


namespace CodeExecutor.Dispatcher.Services.Implementations;

public sealed class ProgrammingLanguagesService : IProgrammingLanguagesService
{
    private readonly DbRepository.ILanguagesRepository context;
    private readonly IMapper mapper;

    public ProgrammingLanguagesService(DbRepository.ILanguagesRepository context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<List<Language>> GetListAsync(int? skip = null, int? take = null)
    {
        List<DbModel.Language> languages = await context.Query()
            .OrderByDescending(e => e.Id)
            .Skip(skip ?? 0)
            .Take(take ?? int.MaxValue)
            .ToListAsync();

        return mapper.Map<List<Language>>(languages)!;
    }

    public async Task<List<Language>> GetVersionsListAsync(string languageName)
    {
        List<DbModel.Language> languages = await context.Query()
            .Where(e => e.Name == languageName)
            .OrderByDescending(e => e.Id)
            .ToListAsync();

        return mapper.Map<List<Language>>(languages)!;
    }
}