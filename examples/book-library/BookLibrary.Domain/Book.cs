using DataExplorer.Abstractions.Entities;
using DataExplorer.Entities;

namespace BookLibrary.Domain;

public class Book : SnowflakeEntity, ICreatedAtOffset, IUpdatedAtOffset, IDisableable
{
    public string Title { get; set; } = default!;
    public DateTimeOffset PublishedAt { get; set; }
    
    public long AuthorId { get; set; }
    public Author? Author { get; set; }
    
    public long PublisherId { get; set; }
    public Publisher? Publisher { get; set; }
    
    private HashSet<Borrowing>? _borrowings;
    public IEnumerable<Borrowing>? Borrowings => _borrowings?.AsEnumerable();
    
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDisabled { get; set; }
}
