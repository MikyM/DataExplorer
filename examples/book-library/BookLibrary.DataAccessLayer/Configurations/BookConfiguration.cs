using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLibrary.DataAccessLayer.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).ValueGeneratedOnAdd().IsRequired();
        
        builder.Property(e => e.Title).IsRequired();
        builder.Property(e => e.PublishedAt).IsRequired();
        
        builder.Property(e => e.CreatedAt).IsRequired(false);
        builder.Property(e => e.UpdatedAt).IsRequired(false);
        builder.Property(e => e.IsDisabled).IsRequired();

        builder.HasMany(e => e.Borrowings)
            .WithOne(e => e.Book)
            .HasForeignKey(e => e.BookId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
    }
}
