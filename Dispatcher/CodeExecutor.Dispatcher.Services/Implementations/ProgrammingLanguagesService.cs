using AutoMapper;
using CodeExecutor.DB.Repository;
using CodeExecutor.Dispatcher.Services.Interfaces;
using LanguageDto = CodeExecutor.Dispatcher.Contracts.Language;


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

    public async Task<List<LanguageDto>> GetListAsync(int? skip = null, int? take = null)
    {
        var languages = await context.Query()
            .OrderByDescending(e => e.Id)
            .Skip(skip ?? 0)
            .Take(take ?? int.MaxValue)
            .ToListAsync();
        return mapper.Map<List<LanguageDto>>(languages);
    }

    public async Task<List<LanguageDto>> GetVersionsListAsync(string languageName)
    {
        var languages = await context.Query()
            .Where(e => e.Name == languageName)
            .OrderByDescending(e => e.Id)
            .ToListAsync();
        return mapper.Map<List<LanguageDto>>(languages);
    }
}