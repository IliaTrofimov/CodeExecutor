using System.ComponentModel.DataAnnotations;

namespace CodeExecutor.DB.Models;

public class Language : BaseEntity<long>
{
    [Required] [StringLength(128)] 
    public string Name { get; set; } = "";
    
    [StringLength(10)] 
    public string? Version { get; set; }

    public override string ToString() => Version is not null 
        ? $"Language(Id={Id}, Name='{Name}-{Version}')"
        : $"Language(Id={Id}, Name='{Name}')";
}