using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLibrary.DataAccessLayer.Configurations;

public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).ValueGeneratedOnAdd().IsRequired();
        
        builder.Property(e => e.Name).IsRequired();
        
        builder.Property(e => e.CreatedAt).IsRequired(false);
        builder.Property(e => e.UpdatedAt).IsRequired(false);
        builder.Property(e => e.IsDisabled).IsRequired();

        builder.HasMany(e => e.Books)
            .WithOne(e => e.Publisher)
            .HasForeignKey(e => e.PublisherId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        builder.Metadata.FindNavigation(nameof(Publisher.Books))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
