using DataExplorer.Abstractions.Entities;
using DataExplorer.Entities;

namespace BookLibrary.Domain;

public class Borrowing : SnowflakeEntity, ICreatedAtOffset, IUpdatedAtOffset, IDisableable
{
    public long ClientId { get; set; }
    public Client? Client { get; set; }
    
    public long BookId { get; set; }
    public Book? Book { get; set; }
   
    public DateTimeOffset BorrowedAt { get; set; }
    public DateTimeOffset? ReturnedAt { get; set; }
    
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDisabled { get; set; }
}
