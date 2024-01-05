using BookLibrary.DataAccessLayer.Configurations;
using DataExplorer;
using DataExplorer.EfCore;
using DataExplorer.EfCore.DataContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BookLibrary.DataAccessLayer;

public class LibraryDbContext : EfDbContext, ILibraryDbContext
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthorConfiguration).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options, IOptions<DataExplorerEfCoreConfiguration> config, DataExplorerTimeProvider timeProvider) : base(options, config, timeProvider)
    {
    }
}
