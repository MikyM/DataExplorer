using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLibrary.DataAccessLayer.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).ValueGeneratedOnAdd().IsRequired();
        
        builder.Property(e => e.FirstName).IsRequired();
        builder.Property(e => e.Surname).IsRequired();
        builder.Property(e => e.Email).IsRequired();
        builder.Property(e => e.PhoneNumber).IsRequired();
        
        builder.Property(e => e.CreatedAt).IsRequired(false);
        builder.Property(e => e.UpdatedAt).IsRequired(false);
        builder.Property(e => e.IsDisabled).IsRequired();

        builder.HasMany(e => e.Borrowings)
            .WithOne(e => e.Client)
            .HasForeignKey(e => e.ClientId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        builder.Metadata.FindNavigation(nameof(Client.Borrowings))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
