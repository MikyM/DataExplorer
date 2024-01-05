using DataExplorer.Abstractions.Entities;
using DataExplorer.Entities;
// ReSharper disable CollectionNeverUpdated.Local
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

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
