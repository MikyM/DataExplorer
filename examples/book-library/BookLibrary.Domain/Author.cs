using DataExplorer.Abstractions.Entities;
using DataExplorer.Entities;

namespace BookLibrary.Domain;

public class Author : SnowflakeEntity, ICreatedAtOffset, IUpdatedAtOffset, IDisableable
{
    public string FirstName { get; set; } = default!;
    public string Surname { get; set; } = default!;
 
    private HashSet<Book>? _books;
    public IEnumerable<Book>? Books => _books?.AsEnumerable();
    
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDisabled { get; set; }
}
