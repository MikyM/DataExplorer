using DataExplorer.EfCore.Specifications.Builders;

namespace BookLibrary.DataAccessLayer.Specifications;

public sealed class AuthorWithBooksSpec : Specification<Author>
{
    public AuthorWithBooksSpec(string surname, string? firstName)
    {
        Where(x => x.Surname.Equals(surname, StringComparison.CurrentCultureIgnoreCase));
        
        if (firstName is not null)
        {
            Where(x => x.FirstName.Equals(firstName, StringComparison.CurrentCultureIgnoreCase));
        }
        
        Where(x => x.IsDisabled == false);
        
        Includes();
    }
    
    public AuthorWithBooksSpec(long id)
    {
        Where(x => x.Id == id);

        Where(x => x.IsDisabled == false);
        
        Includes();
    }
    
    private void Includes()
    {
        Include(x => x.Books)
            .ThenInclude(x => x!.Publisher);
    }
}
