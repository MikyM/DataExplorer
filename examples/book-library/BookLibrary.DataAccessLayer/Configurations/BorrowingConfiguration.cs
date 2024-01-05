using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLibrary.DataAccessLayer.Configurations;

public class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
{
    public void Configure(EntityTypeBuilder<Borrowing> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).ValueGeneratedOnAdd().IsRequired();
        
        builder.Property(e => e.BorrowedAt).IsRequired();
        builder.Property(e => e.ReturnedAt).IsRequired(false);
        
        builder.Property(e => e.CreatedAt).IsRequired(false);
        builder.Property(e => e.UpdatedAt).IsRequired(false);
        builder.Property(e => e.IsDisabled).IsRequired();
    }
}
