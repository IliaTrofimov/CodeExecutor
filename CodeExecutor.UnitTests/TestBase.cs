using AutoMapper;
using CodeExecutor.Dispatcher.Services.Interfaces;
using CodeExecutor.Dispatcher.Services.Utils;
using CodeExecutor.UnitTests.Mocks.Repositories;
using CodeExecutor.UnitTests.Mocks.Services;
using Xunit.Abstractions;

namespace CodeExecutor.UnitTests;

public abstract class TestBase
{
    protected readonly DBRepo.IUsersRepository UsersRepository;
    protected readonly DBRepo.ILanguagesRepository LanguagesRepository;
    protected readonly DBRepo.ICodeExecutionsEditorRepository ExecutionsEditorRepository;
    protected readonly DBRepo.ICodeExecutionsExplorerRepository ExecutionsExplorerRepository;
    
    protected readonly ICodeExecutionMessaging ExecutionMessaging;

    protected readonly ITestOutputHelper Output;
    protected readonly IMapper Mapper;
    
    
    protected TestBase(ITestOutputHelper output)
    {
        Output = output;
        
        UsersRepository = new UsersRepositoryMock(new TestLogger<UsersRepositoryMock>(output));
        ExecutionMessaging = new CodeExecutionMqMock(new TestLogger<CodeExecutionMqMock>(output));
        LanguagesRepository = new LanguagesRepositoryMock(new TestLogger<LanguagesRepositoryMock>(output));

        var executionsMock = new CodeExecutionRepositoryMock(new TestLogger<CodeExecutionRepositoryMock>(output));
        ExecutionsEditorRepository = executionsMock;
        ExecutionsExplorerRepository = executionsMock;
        
        Mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile())).CreateMapper();
    }
}