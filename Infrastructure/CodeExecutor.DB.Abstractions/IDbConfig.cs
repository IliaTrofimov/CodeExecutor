namespace CodeExecutor.DB.Abstractions;

public interface IDbConfig
{
    string ConnectionString { get; }
}