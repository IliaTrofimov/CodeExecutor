using CodeExecutor.Common.Models.Exceptions;
using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Dispatcher.Services.Implementations;
using CodeExecutor.Dispatcher.Services.Interfaces;
using CodeExecutor.UnitTests.Mocks.Repositories;
using CodeExecutor.UnitTests.Mocks.Services;
using Xunit.Abstractions;


namespace CodeExecutor.UnitTests;

public class CodeExecutionModificationTests : DbTestBase
{
    protected const long userId = 1;

    protected ICodeExecutionDispatcher ExecutionDispatcher;
    protected ICodeExecutionExplorer ExecutionExplorer;


    public CodeExecutionModificationTests(ITestOutputHelper output) : base(output, TestDbType.InMemory)
    {
        ExecutionDispatcher = new CodeExecutionDispatcher(ExecutionsExplorerRepository,
            ExecutionsEditorRepository,
            LanguagesRepository,
            ExecutionMessaging,
            new TestLogger<CodeExecutionDispatcher>(output),
            Mapper);

        ExecutionExplorer = new CodeExecutionExplorer(ExecutionsExplorerRepository, Mapper);
    }
    
    private async Task<Guid> StartExecutionInternal()
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
        Assert.False(db.IsError);
        Assert.NotEqual(new DateTimeOffset(), db.RequestedAt);
        Assert.Equal(db.RequestedAt, db.UpdatedAt);
        Assert.Equal(request.LanguageId, db.LanguageId);
        Assert.Equal(userId, db.InitiatorId);
        Assert.Equal(request.CodeText, db.SourceCode?.CodeText);

        return response.Guid;
    }

    
    [Theory]
    [InlineData(false, "new data", null)]
    [InlineData(true, "new data", null)]
    [InlineData(false, null, "new comment")]
    [InlineData(true, null, "new comment")]
    [InlineData(null, "new data", "new comment")]
    [InlineData(true, "new data", "new comment")]
    public async Task SetExecutionResults(bool? isError, string? data, string? comment)
    {
        var guid = await StartExecutionInternal();
        var db = await ExecutionsExplorerRepository.GetAsync(guid);
        Assert.NotNull(db);

        var updatedAt = db.UpdatedAt;
        var dbIsError = db.IsError;
        var dbData = db.Result?.Data;
        var dbComment = db.Comment;

        var executionResult = new CodeExecutionResult
        {
            Guid = guid,
            IsError = isError,
            Data = data,
            Status = isError == true ? CodeExecutionStatus.Error : CodeExecutionStatus.Finished,
            Comment = comment
        };

        await Task.Delay(200);
        await ExecutionDispatcher.SetExecutionResultsAsync(executionResult, db.SecretKey);

        var dbUpdated = await ExecutionsExplorerRepository.GetAsync(guid);
        Assert.NotNull(dbUpdated);

        Assert.Equal(isError ?? dbIsError, dbUpdated.IsError);
        Assert.Equal(data ?? dbData, dbUpdated.Result?.Data);
        Assert.Equal(comment ?? dbComment, dbUpdated.Comment);
        Assert.True(updatedAt < dbUpdated.UpdatedAt, "updatedAt < dbUpdated.UpdatedAt");
    }

    [Theory]
    [InlineData(true, CodeExecutionStatus.Error)]
    [InlineData(true, CodeExecutionStatus.Pending)]
    [InlineData(false, CodeExecutionStatus.None)]
    [InlineData(false, CodeExecutionStatus.Started)]
    [InlineData(false, CodeExecutionStatus.Finished)]
    [InlineData(false, CodeExecutionStatus.Pending)]
    public async Task SetExecutionResultsCheckStatus(bool? isError, CodeExecutionStatus status)
    {
        var guid = await StartExecutionInternal();
        var db = await ExecutionsExplorerRepository.GetAsync(guid);
        Assert.NotNull(db);

        var updatedAt = db.UpdatedAt;
        var requestedAt = db.RequestedAt;
        DateTimeOffset? startedAtAt = db.StartedAt;
        DateTimeOffset? finishedAt = db.FinishedAt;

        var executionResult = new CodeExecutionResult
        {
            Guid = guid,
            IsError = isError,
            Data = "new data",
            Status = status,
            Comment = "new comment"
        };

        await Task.Delay(200);
        await ExecutionDispatcher.SetExecutionResultsAsync(executionResult, db.SecretKey);

        var dbUpdated = await ExecutionsExplorerRepository.GetAsync(guid);
        Assert.NotNull(dbUpdated);
        Assert.True(updatedAt < dbUpdated.UpdatedAt, "updatedAt < dbUpdated.UpdatedAt");

        switch (status)
        {
            case CodeExecutionStatus.Error or CodeExecutionStatus.Finished:
                Assert.NotNull(dbUpdated.FinishedAt);
                Assert.True(updatedAt < dbUpdated.FinishedAt, "updatedAt < dbUpdated.FinishedAt");
                break;
            case CodeExecutionStatus.Started:
                Assert.Null(dbUpdated.FinishedAt);
                Assert.NotNull(dbUpdated.StartedAt);
                Assert.True(updatedAt < dbUpdated.StartedAt, "updatedAt < dbUpdated.StartedAt");
                break;
        }
    }

    [Fact]
    public async Task SetExecutionResultsEmpty()
    {
        var guid = await StartExecutionInternal();
        var db = await ExecutionsExplorerRepository.GetAsync(guid);
        Assert.NotNull(db);

        var updatedAt = db.UpdatedAt;
        var dbIsError = db.IsError;
        var dbData = db.Result?.Data;
        var dbComment = db.Comment;

        var executionResult = new CodeExecutionResult { Guid = guid };

        await Task.Delay(200);
        await ExecutionDispatcher.SetExecutionResultsAsync(executionResult, db.SecretKey);

        var dbUpdated = await ExecutionsExplorerRepository.GetAsync(guid);
        Assert.NotNull(dbUpdated);

        Assert.Equal(dbIsError, dbUpdated.IsError);
        Assert.Equal(dbData, dbUpdated.Result?.Data);
        Assert.Equal(dbComment, dbUpdated.Comment);
        Assert.Equal(updatedAt, dbUpdated.UpdatedAt);
    }

    [Fact]
    public async Task SetExecutionResultsWrongValidationTag()
    {
        var guid = await StartExecutionInternal();
        var db = await ExecutionsExplorerRepository.GetAsync(guid);
        Assert.NotNull(db);

        var updatedAt = db.UpdatedAt;
        var dbIsError = db.IsError;
        var dbData = db.Result?.Data;
        var dbComment = db.Comment;

        var executionResult = new CodeExecutionResult
        {
            Guid = guid,
            IsError = true
        };

        await Task.Delay(200);
        var ex = await Assert.ThrowsAsync<UnauthorizedException>(async () =>
            await ExecutionDispatcher.SetExecutionResultsAsync(executionResult, "x"));

        Assert.Contains("Cannot change", ex.Message);

        var dbUpdated = await ExecutionsExplorerRepository.GetAsync(guid);
        Assert.NotNull(dbUpdated);

        Assert.Equal(dbIsError, dbUpdated.IsError);
        Assert.Equal(dbData, dbUpdated.Result?.Data);
        Assert.Equal(dbComment, dbUpdated.Comment);
        Assert.Equal(updatedAt, dbUpdated.UpdatedAt);
    }
}