using AutoMapper;
using CodeExecutor.Dispatcher.Services.Interfaces;
using CodeExecutor.Dispatcher.Services.Utils;
using CodeExecutor.UnitTests.Mocks.Services;
using Xunit.Abstractions;


namespace CodeExecutor.UnitTests;


public abstract class TestBase
{
    protected readonly ICodeExecutionMessaging ExecutionMessaging;
    protected readonly IMapper Mapper;
    protected readonly TestLogger Output;

    protected TestBase(ITestOutputHelper output)
    {
        Output = new TestLogger<TestBase>(output);
        ExecutionMessaging = new CodeExecutionMqMock(new TestLogger<CodeExecutionMqMock>(output));
        Mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile())).CreateMapper();
    }
}