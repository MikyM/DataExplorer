using DataExplorer.EfCore.Specifications.Builders;

namespace BookLibrary.DataAccessLayer.Specifications;

public sealed class PublisherWithBooksSpec : Specification<Publisher>
{
    public PublisherWithBooksSpec(string name)
    {
        Where(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

        Where(x => x.IsDisabled == false);
        
        Includes();
    }
    
    public PublisherWithBooksSpec(long id)
    {
        Where(x => x.Id == id);
        
        Where(x => x.IsDisabled == false);
        
        Includes();
    }
    
    private void Includes()
    {
        Include(x => x.Books)
            .ThenInclude(x => x!.Author);
    }
}
