using CodeExecutor.DB;
using CodeExecutor.DB.Repository;
using CodeExecutor.UnitTests.Mocks.Repositories;
using CodeExecutor.UnitTests.Mocks.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;


namespace CodeExecutor.UnitTests;

public enum TestDbType { Mock, InMemory };


public abstract class DbTestBase : TestBase, IDisposable, IAsyncDisposable
{
    protected readonly DataContext? DatabaseContext;
    protected readonly TestDbType DbType;
    
    protected readonly DBRepo.IUsersRepository UsersRepository;
    protected readonly DBRepo.ILanguagesRepository LanguagesRepository;
    protected readonly DBRepo.ICodeExecutionsEditorRepository ExecutionsEditorRepository;
    protected readonly DBRepo.ICodeExecutionsExplorerRepository ExecutionsExplorerRepository;

    
    protected DbTestBase(ITestOutputHelper output, TestDbType dbType)
        : base(output)
    {
        DbType = dbType;
        if (dbType is TestDbType.InMemory)
        {
            DatabaseContext = CreateDataContext(); 
            UsersRepository = new UsersRepository(DatabaseContext);
            LanguagesRepository = new LanguagesRepository(DatabaseContext);
            var executionsRepo = new CodeExecutionRepository(DatabaseContext);
            ExecutionsEditorRepository = executionsRepo;
            ExecutionsExplorerRepository = executionsRepo;
        }
        else
        {
            UsersRepository = new UsersRepositoryMock(new TestLogger<UsersRepositoryMock>(output));
            LanguagesRepository = new LanguagesRepositoryMock(new TestLogger<LanguagesRepositoryMock>(output));
            var executionsMock = new CodeExecutionRepositoryMock(new TestLogger<CodeExecutionRepositoryMock>(output));
            ExecutionsEditorRepository = executionsMock;
            ExecutionsExplorerRepository = executionsMock;
        }
        
        InitDataBase();
        Output.LogInformation("Starting test with {dbType} database",dbType);
    }

    protected virtual void InitDataBase()
    {
        if (DbType is TestDbType.InMemory && DatabaseContext is not null)
        {
            Output.LogInformation("Initializing database");
            if (DatabaseContext.Languages.Count() < 3)
            {
                DatabaseContext.Languages.Add(new DBModels.Language
                {
                    Name = "CSharp",
                    Version = "12"
                });
                DatabaseContext.Languages.Add(new DBModels.Language
                {
                    Name = "Python",
                    Version = "10"
                });
                DatabaseContext.Languages.Add(new DBModels.Language
                {
                    Name = "PascalABC.Net",
                });
            }

            if (!DatabaseContext.Users.Any())
            {
                DatabaseContext.Users.Add(new DBModels.User
                {
                    Username = "testUser",
                    PasswordHash = Enumerable.Repeat((byte)128, 32).ToArray()
                }); 
            }
            
            DatabaseContext.SaveChanges();
        }
    }
    
    private static DataContext CreateDataContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "Test_Database")
            .Options;
        return new DataContext(options);
    }


    public void Dispose()
    {
        Output.LogInformation("Disposing test class");
        DatabaseContext?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        Output.LogInformation("Disposing test class");
        if (DatabaseContext is not null)
        {
            await DatabaseContext.DisposeAsync();
        }
    }

    protected void ClearDatabase()
    {
        if (DatabaseContext is not null)
        {
            DatabaseContext.Languages.RemoveRange(DatabaseContext.Languages);
            DatabaseContext.Users.RemoveRange(DatabaseContext.Users);    
            DatabaseContext.CodeExecutions.RemoveRange(DatabaseContext.CodeExecutions);    
            DatabaseContext.SourceCodes.RemoveRange(DatabaseContext.SourceCodes);    
            DatabaseContext.ExecutionResults.RemoveRange(DatabaseContext.ExecutionResults);    
            DatabaseContext.SaveChanges();
        }
    }
}