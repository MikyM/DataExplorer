using DataExplorer.Abstractions.Entities;
using DataExplorer.Entities;

namespace BookLibrary.Domain;

public class Client : SnowflakeEntity, ICreatedAtOffset, IUpdatedAtOffset, IDisableable
{
    public string FirstName { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Email { get; set; } = default!;
    
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDisabled { get; set; }
    
    private HashSet<Borrowing>? _borrowings;
    public IEnumerable<Borrowing>? Borrowings => _borrowings?.AsEnumerable();
}
