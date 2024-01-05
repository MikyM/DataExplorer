using DataExplorer.EfCore.Specifications.Builders;

namespace BookLibrary.DataAccessLayer.Specifications;

public sealed class BorrowingWithInfoSpec : Specification<Borrowing>
{
    public BorrowingWithInfoSpec(long id)
    {
        Where(x => x.Id == id);

        Where(x => x.IsDisabled == false);
        
        Includes();
    }
    
    private void Includes()
    {
        Include(x => x.Book)
            .ThenInclude(x => x!.Author);
        Include(x => x.Book)
            .ThenInclude(x => x!.Publisher);
        Include(x => x.Client);
    }
}
