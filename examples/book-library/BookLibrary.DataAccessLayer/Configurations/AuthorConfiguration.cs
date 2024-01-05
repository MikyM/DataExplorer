using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLibrary.DataAccessLayer.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).ValueGeneratedOnAdd().IsRequired();
        
        builder.Property(e => e.FirstName).IsRequired();
        builder.Property(e => e.Surname).IsRequired();
        
        
        builder.Property(e => e.CreatedAt).IsRequired(false);
        builder.Property(e => e.UpdatedAt).IsRequired(false);
        builder.Property(e => e.IsDisabled).IsRequired();

        builder.HasMany(e => e.Books)
            .WithOne(e => e.Author)
            .HasForeignKey(e => e.AuthorId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        builder.Metadata.FindNavigation(nameof(Author.Books))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
