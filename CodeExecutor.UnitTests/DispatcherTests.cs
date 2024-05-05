using AutoMapper;
using CodeExecutor.Common.Models.Exceptions;
using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Dispatcher.Host.Services.Implementations;
using CodeExecutor.Dispatcher.Host.Services.Interfaces;
using CodeExecutor.UnitTests.Mocks.Repositories;
using CodeExecutor.UnitTests.Mocks.Services;
using Xunit.Abstractions;

namespace CodeExecutor.UnitTests;

public class DispatcherTests : TestBase
{
    protected const long userId = 1;
    
    protected ICodeExecutionDispatcher ExecutionDispatcher;
    protected ICodeExecutionExplorer ExecutionExplorer;


    public DispatcherTests(ITestOutputHelper output) : base(output)
    {
        ExecutionDispatcher = new CodeExecutionDispatcher(ExecutionsExplorerRepository,
            ExecutionsEditorRepository, 
            LanguagesRepository, 
            ExecutionMessaging,
            new TestLogger<CodeExecutionDispatcher>(output),
            Mapper);
        
        ExecutionExplorer = new CodeExecutionExplorer(ExecutionsExplorerRepository, Mapper);
    }
    
    [Fact]
    public async Task<Guid> StartExecutionDefault()
    {
        var request = new CodeExecutionRequest
        {
            LanguageId = LanguagesRepositoryMock.CSharpId,
            CodeText = "hello world",
            Priority = ExecutionPriority.High
        };

        var response = await ExecutionDispatcher.StartCodeExecutionAsync(request, userId);
        Assert.NotNull(response);
        Assert.NotEqual(new Guid(), response.Guid);
        
        var db = await ExecutionsExplorerRepository.GetAsync(response.Guid);
        Assert.NotNull(db);
        Assert.Equal(false, db.IsError);
        Assert.NotEqual(new DateTimeOffset(), db.RequestedAt);
        Assert.Equal(db.RequestedAt, db.UpdatedAt);
        Assert.Equal(request.LanguageId, db.LanguageId);
        Assert.Equal(userId, db.InitiatorId);
        Assert.Equal(request.CodeText, db.SourceCode?.CodeText);

        return response.Guid;
    }
    
    [Fact]
    public async Task StartExecutionMissingLanguage()
    {
        var request = new CodeExecutionRequest
        {
            LanguageId = LanguagesRepositoryMock.Missing,
            CodeText = "hello world",
            Priority = ExecutionPriority.High
        };
        var ex = await Assert.ThrowsAsync<BadRequestException>(async () => 
            await ExecutionDispatcher.StartCodeExecutionAsync(request, userId));

        Assert.Contains("Language", ex.Message);
        Assert.Contains("not exists", ex.Message);
    }
    
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetExecutionResults(bool exists)
    {
        if (exists)
        {
            var guid = await StartExecutionDefault();
            var db = await ExecutionsExplorerRepository.GetAsync(guid);
            Assert.NotNull(db?.Result);
            db.Result.Data = "data\ndata";
            
            var dto = await ExecutionExplorer.GetExecutionResultAsync(guid, userId);
            Assert.NotNull(dto);

            Assert.Equal(db.IsError, dto.IsError);
            Assert.Equal(db.Comment, dto.Comment);
            Assert.Equal(db.UpdatedAt, dto.UpdatedAt);
            Assert.Equal(db.RequestedAt, dto.RequestedAt);
            Assert.Equal(db.StartedAt, dto.StartedAt);
            Assert.Equal(db.FinishedAt, dto.FinishedAt);
            Assert.Equal(db.Result.Data, dto.Data);
        }
        else
        {
            var dto = await ExecutionExplorer.GetExecutionResultAsync(new Guid(), userId);
            Assert.Null(dto);
        }
    }
    
    [Fact]
    public async Task GetExecutionResultsWrongUser()
    {
        var guid = await StartExecutionDefault();
        var ex = await Assert.ThrowsAsync<UnauthorizedException>(async () => 
            await ExecutionExplorer.GetExecutionResultAsync(guid, userId + 1));
        
        Assert.Contains("Unauthorized", ex.Message);
        Assert.Contains("Cannot view this execution", ex.Message);
    }
    
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetExecutionSources(bool exists)
    {
        if (exists)
        {
            var guid = await StartExecutionDefault();
            var db = await ExecutionsExplorerRepository.GetAsync(guid);
            Assert.NotNull(db?.SourceCode);
            
            var dto = await ExecutionExplorer.GetSourceCodeAsync(guid, userId);
            Assert.NotNull(dto);

            Assert.Equal(guid, dto.Guid);
            Assert.Equal(db.LanguageId, dto.LanguageId);
            Assert.Equal(db.SourceCode.CodeText, dto.CodeText);
        }
        else
        {
            var dto = await ExecutionExplorer.GetSourceCodeAsync(new Guid(), userId);
            Assert.Null(dto);
        }
    }
    
    [Fact]
    public async Task GetExecutionSourcesWrongUser()
    {
        var guid = await StartExecutionDefault();
        var ex = await Assert.ThrowsAsync<UnauthorizedException>(async () => 
            await ExecutionExplorer.GetSourceCodeAsync(guid, userId + 1));
        
        Assert.Contains("Unauthorized", ex.Message);
        Assert.Contains("Cannot view this execution", ex.Message);
    }
}