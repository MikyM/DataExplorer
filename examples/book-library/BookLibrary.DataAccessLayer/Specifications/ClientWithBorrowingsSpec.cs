using DataExplorer.EfCore.Specifications.Builders;

namespace BookLibrary.DataAccessLayer.Specifications;

public sealed class ClientWithBorrowingsSpec : Specification<Client>
{
    public ClientWithBorrowingsSpec(long clientId)
    {
        Where(x => x.Id == clientId);
        
        Where(x => x.IsDisabled == false);
        
        Include(x => x.Borrowings)
            .ThenInclude(x => x!.Book)
            .ThenInclude(x => x!.Author);
    }
}
