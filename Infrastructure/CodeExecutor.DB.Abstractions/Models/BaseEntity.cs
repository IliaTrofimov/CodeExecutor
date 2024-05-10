using System.ComponentModel.DataAnnotations;


namespace CodeExecutor.DB.Abstractions.Models;

/// <summary>Database entity with Id column.</summary>
/// <typeparam name="TKey">Primary key type.</typeparam>
public interface IEntity<out TKey> where TKey : notnull
{
    /// <summary>Primary key.</summary>
    public TKey Id { get; }
}


/// <summary>Base abstract class for all database entities with Id.</summary>
/// <typeparam name="TKey">Primary key type.</typeparam>
public abstract class BaseEntity<TKey> : IEntity<TKey>
    where TKey : notnull
{
    [Key] public TKey Id { get; set; } = default!;

    public override string ToString() => $"{GetType().Name}(Id={Id})";
}