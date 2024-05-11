#region

using CodeExecutor.Dispatcher.Contracts;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

#endregion

namespace CodeExecutor.UnitTests;

public class MapperTests(ITestOutputHelper output) : TestBase(output)
{
    private static (DBModels.CodeExecution db, CodeExecution dto) GetCodeExecutions(bool nullResults = false, bool nullSource = false)
    {
        var languages = GetLanguages();
        var id = Guid.NewGuid();
        var db = new DBModels.CodeExecution
        {
            Id = id,
            LanguageId = languages.db.Id,
            Language = languages.db,
            RequestedAt = new DateTimeOffset(),
            UpdatedAt = new DateTimeOffset().AddSeconds(10),
            StartedAt = new DateTimeOffset().AddSeconds(5),
            FinishedAt = new DateTimeOffset().AddSeconds(20),
            IsError = false,
            Comment = "comment",
            Result = nullResults
                ? null
                : new DBModels.CodeExecutionResult
                {
                    Id = id,
                    Data = "data data\ndata"
                },
            SourceCode = nullSource
                ? null
                : new DBModels.SourceCode
                {
                    Id = id,
                    CodeText = "source\ncode"
                }
        };

        var dto = new CodeExecutionExpanded
        {
            Guid = db.Id,
            Language = languages.dto,
            RequestedAt = db.RequestedAt,
            UpdatedAt = db.UpdatedAt,
            StartedAt = db.StartedAt,
            FinishedAt = db.FinishedAt,
            IsError = db.IsError,
            Comment = db.Comment,
            Data = db.Result?.Data,
            SourceCode = db.SourceCode?.CodeText
        };

        return (db, dto);
    }

    private static (DBModels.Language db, Language dto) GetLanguages()
    {
        var db = new DBModels.Language
        {
            Id = 10,
            Name = "language",
            Version = "version"
        };

        var dto = new Language
        {
            Id = db.Id,
            Name = db.Name,
            Version = db.Version
        };

        return (db, dto);
    }


    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void CodeExecutionMapping(bool nullResults, bool nullSource)
    {
        var (db, _) = GetCodeExecutions(nullResults, nullSource);
        var dtoSmall = Mapper.Map<CodeExecution>(db);

        Assert.NotNull(dtoSmall);
        Assert.Equal(db.Id, dtoSmall.Guid);
        Assert.Equal(db.RequestedAt, dtoSmall.RequestedAt);
        Assert.Equal(db.StartedAt, dtoSmall.StartedAt);
        Assert.Equal(db.UpdatedAt, dtoSmall.UpdatedAt);
        Assert.Equal(db.FinishedAt, dtoSmall.FinishedAt);
        Assert.Equal(db.Comment, dtoSmall.Comment);
        Assert.Equal(db.LanguageId, dtoSmall.Language.Id);
        Assert.Equal(db.Language.Id, dtoSmall.Language.Id);
        Assert.Equal(db.Language.Name, dtoSmall.Language.Name);
        Assert.Equal(db.Language.Version, dtoSmall.Language.Version);

        Output.LogInformation("Mapper.Map<CodeExecution>() IS SUCCESSFUL");
        var dtoExpanded = Mapper.Map<CodeExecutionExpanded>(db);

        Assert.NotNull(dtoExpanded);
        Assert.Equal(db.Id, dtoExpanded.Guid);
        Assert.Equal(db.RequestedAt, dtoExpanded.RequestedAt);
        Assert.Equal(db.StartedAt, dtoExpanded.StartedAt);
        Assert.Equal(db.UpdatedAt, dtoExpanded.UpdatedAt);
        Assert.Equal(db.FinishedAt, dtoExpanded.FinishedAt);
        Assert.Equal(db.Comment, dtoExpanded.Comment);
        Assert.Equal(db.Result?.Data, dtoExpanded.Data);
        Assert.Equal(db.SourceCode?.CodeText, dtoExpanded.SourceCode);
        Assert.Equal(db.LanguageId, dtoSmall.Language.Id);
        Assert.Equal(db.Language.Id, dtoSmall.Language.Id);
        Assert.Equal(db.Language.Name, dtoSmall.Language.Name);
        Assert.Equal(db.Language.Version, dtoSmall.Language.Version);

        Output.LogInformation("Mapper.Map<CodeExecutionExpanded>() IS SUCCESSFUL");
    }

    [Fact]
    public void LanguageMapping()
    {
        var (db, _) = GetLanguages();
        var dto = Mapper.Map<Language>(db);

        Assert.NotNull(dto);
        Assert.Equal(db.Id, dto.Id);
        Assert.Equal(db.Name, dto.Name);
        Assert.Equal(db.Version, dto.Version);
    }

    [Fact]
    public void CodeExecutionSecretKey()
    {
        var (db, _) = GetCodeExecutions();
        Assert.NotNull(db.SecretKey);
        Assert.NotEmpty(db.SecretKey);
    }
}