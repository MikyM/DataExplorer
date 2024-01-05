namespace BookLibrary.DataAccessLayer.Specifications;

public sealed class PutClientSpec : UpdateSpecification<Client>
{
    public PutClientSpec(long id, Client client)
    {
        Where(x => x.Id == id);

        Where(x => x.IsDisabled == false);
        
        Modify(x =>
            x.SetProperty(p => p.Surname, client.Surname)
                .SetProperty(p => p.FirstName, client.FirstName)
                .SetProperty(p => p.Email, client.Email)
                .SetProperty(p => p.PhoneNumber, client.PhoneNumber));
    }
}
