using DataExplorer.EfCore.Specifications.Builders;

namespace BookLibrary.DataAccessLayer.Specifications;

public sealed class BookWithInfoSpec : Specification<Book>
{
    public BookWithInfoSpec(string title)
    {
        Where(x => x.Title.Equals(title, StringComparison.CurrentCultureIgnoreCase));

        Where(x => x.IsDisabled == false);
        
        Includes();
    }

    public BookWithInfoSpec(long id)
    {
        Where(x => x.Id == id);

        Where(x => x.IsDisabled == false);
        
        Includes();
    }
    
    private void Includes()
    {
        Include(x => x.Author);
        Include(x => x.Publisher);
        Include(x => x.Borrowings)
            .ThenInclude(x => x!.Client);
    }
}
