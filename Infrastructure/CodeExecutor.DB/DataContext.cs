using CodeExecutor.DB.Abstractions.Models;
using Microsoft.EntityFrameworkCore;


namespace CodeExecutor.DB;

/// <summary>Database context for code execution requests.</summary>
public class DataContext : DbContext
{
    /// <summary>All code execution requests.</summary>
    public DbSet<CodeExecution> CodeExecutions { get; set; } = null!;

    /// <summary>All code execution requests source codes.</summary>
    public DbSet<SourceCode> SourceCodes { get; set; } = null!;

    /// <summary>All code execution results.</summary>
    public DbSet<CodeExecutionResult> ExecutionResults { get; set; } = null!;

    /// <summary>Application users.</summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>Available programming languages.</summary>
    public DbSet<Language> Languages { get; set; } = null!;


    //public DataContext(): base() {}

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CodeExecution>()
            .HasOne(e => e.SourceCode)
            .WithOne(s => s.CodeExecution)
            .HasForeignKey<SourceCode>(s => s.Id)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<CodeExecution>()
            .HasOne(e => e.Result)
            .WithOne(s => s.CodeExecution)
            .HasForeignKey<CodeExecutionResult>(s => s.Id)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<CodeExecution>()
            .HasOne(e => e.Language)
            .WithMany()
            .HasForeignKey(s => s.LanguageId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username);

        SeedData(modelBuilder);
    }

    protected void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Language>().HasData(
            new Language { Id = 1, Name = "CSharp", Version = "12" },
            new Language { Id = 2, Name = "Python", Version = "10" });
    }
}