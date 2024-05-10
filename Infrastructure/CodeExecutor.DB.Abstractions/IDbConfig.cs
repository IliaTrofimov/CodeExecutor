namespace CodeExecutor.DB.Abstractions;

/// <summary>Database connection configuration.</summary>
public interface IDbConfig
{
    /// <summary>Database connection string.</summary>
    string ConnectionString { get; }
}