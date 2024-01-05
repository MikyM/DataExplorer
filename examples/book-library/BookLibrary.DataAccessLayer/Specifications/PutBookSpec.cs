namespace BookLibrary.DataAccessLayer.Specifications;

public sealed class PutBookSpec : UpdateSpecification<Book>
{
    public PutBookSpec(long id, Book book)
    {
        Where(x => x.Id == id);

        Where(x => x.IsDisabled == false);
        
        Modify(x =>
            x.SetProperty(p => p.Title, book.Title)
                .SetProperty(p => p.AuthorId, book.AuthorId)
                .SetProperty(p => p.PublisherId, book.PublisherId)
                .SetProperty(p => p.UpdatedAt, DateTimeOffset.UtcNow));
    }
}
