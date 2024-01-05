using DataExplorer.EfCore.Specifications.Builders;

namespace BookLibrary.DataAccessLayer.Specifications;

public sealed class ClientWithBorrowingsSpec : Specification<Client>
{
    public ClientWithBorrowingsSpec(long clientId)
    {
        Where(x => x.Id == clientId);
        
        Where(x => x.IsDisabled == false);
        
        Includes();
    }
    
    public ClientWithBorrowingsSpec(string? firstName, string surname)
    {
        Where(x => x.Surname.Equals(surname, StringComparison.CurrentCultureIgnoreCase));
        
        if (firstName is not null)
        {
            Where(x => x.FirstName.Equals(firstName, StringComparison.CurrentCultureIgnoreCase));
        }
        
        Where(x => x.IsDisabled == false);
        
        Includes();
    }

    private void Includes()
    {
        Include(x => x.Borrowings)
            .ThenInclude(x => x!.Book)
            .ThenInclude(x => x!.Author);
    }
}
