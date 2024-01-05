namespace BookLibrary.DataAccessLayer.Specifications;

public sealed class PutPublisherSpec : UpdateSpecification<Publisher>
{
    public PutPublisherSpec(long id, Publisher publisher)
    {
        Where(x => x.Id == id);
        
        Where(x => x.IsDisabled == false);
        
        Modify(x =>
            x.SetProperty(p => p.Name, publisher.Name)
                .SetProperty(p => p.UpdatedAt, DateTimeOffset.UtcNow));
    }
}
