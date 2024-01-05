namespace BookLibrary.DataAccessLayer.Specifications;

public sealed class ReturnBorrowingSpec : UpdateSpecification<Borrowing>
{
    public ReturnBorrowingSpec(long id, DateTimeOffset returnedAt)
    {
        Where(x => x.Id == id);
        
        Modify(x =>
            x.SetProperty(p => p.ReturnedAt, returnedAt)
                .SetProperty(p => p.UpdatedAt, DateTimeOffset.UtcNow));
    }
}
