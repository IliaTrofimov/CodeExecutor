using CodeExecutor.Common.Models.Exceptions;
using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Dispatcher.Services.Implementations;
using CodeExecutor.Dispatcher.Services.Interfaces;
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


    [Theory]
    [InlineData(false, "new data", null)]
    [InlineData(true, "new data", null)]
    [InlineData(false, null, "new comment")]
    [InlineData(true, null, "new comment")]
    [InlineData(null, "new data", "new comment")]
    [InlineData(true, "new data", "new comment")]
    public async Task SetExecutionResults(bool? isError, string? data, string? comment)
    {
        var guid = await StartExecutionDefault();
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

        await Task.Delay(1000);
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
    [InlineData(false, null)]
    [InlineData(false, CodeExecutionStatus.Started)]
    [InlineData(false, CodeExecutionStatus.Finished)]
    [InlineData(false, CodeExecutionStatus.Pending)]
    public async Task SetExecutionResultsCheckStatus(bool? isError, CodeExecutionStatus? status,
                                                     CodeExecutionStatus? prevStatus = null)
    {
        var guid = await StartExecutionDefault();
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

        await Task.Delay(1000);
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
        var guid = await StartExecutionDefault();
        var db = await ExecutionsExplorerRepository.GetAsync(guid);
        Assert.NotNull(db);

        var updatedAt = db.UpdatedAt;
        var dbIsError = db.IsError;
        var dbData = db.Result?.Data;
        var dbComment = db.Comment;

        var executionResult = new CodeExecutionResult { Guid = guid };

        await Task.Delay(1000);
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
        var guid = await StartExecutionDefault();
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

        await Task.Delay(1000);
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