using DataExplorer.Abstractions.Entities;
using DataExplorer.Entities;

// ReSharper disable CollectionNeverUpdated.Local
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace BookLibrary.Domain;

public class Publisher : SnowflakeEntity, ICreatedAtOffset, IUpdatedAtOffset, IDisableable
{
    public string Name { get; set; } = default!;
    
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDisabled { get; set; }
    
    private HashSet<Book>? _books;
    public IEnumerable<Book>? Books => _books?.AsEnumerable();
}
