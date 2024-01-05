using DataExplorer.Abstractions.Entities;
using DataExplorer.Entities;

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
