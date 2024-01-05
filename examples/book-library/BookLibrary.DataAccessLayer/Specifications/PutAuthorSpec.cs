namespace BookLibrary.DataAccessLayer.Specifications;

public sealed class PutAuthorSpec : UpdateSpecification<Author>
{
    public PutAuthorSpec(long id, Author author)
    {
        Where(x => x.Id == id);

        Where(x => x.IsDisabled == false);
        
        Modify(x =>
            x.SetProperty(p => p.Surname, author.Surname)
                .SetProperty(p => p.FirstName, author.FirstName)
                .SetProperty(p => p.UpdatedAt, DateTimeOffset.UtcNow));
    }
}
