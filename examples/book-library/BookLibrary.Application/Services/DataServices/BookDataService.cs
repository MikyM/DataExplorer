using BookLibrary.Application.Models;
using BookLibrary.Application.Services.DataServices.Abstractions;
using BookLibrary.DataAccessLayer;
using BookLibrary.Domain;
using DataExplorer.EfCore.Abstractions;
using DataExplorer.EfCore.Abstractions.Repositories;
using Microsoft.Extensions.Logging;
using Remora.Results;

namespace BookLibrary.Application.Services.DataServices;

[UsedImplicitly]
public class BookDataService : CrudDataService<Book, ILibraryDbContext>, IBookDataService
{
    private readonly ILogger<BookDataService> _logger;
    
    public BookDataService(IUnitOfWork<ILibraryDbContext> uof, ILogger<BookDataService> logger) : base(uof)
    {
        _logger = logger;
    }

    public async Task<Result<Book>> AddBookAsync(AddBookRequest request)
    {
        try
        {
            var repo1 = UnitOfWork.GetRepository<IRepository<Book>>(); // these are going to be same instances as the data service grabs the entity corresponding repository from the UoF upon creation
            //var repo2 = base.Repository;
        
            var mapped = Mapper.Map<Book>(request);
            
            await repo1.AddAsync(mapped);

            await UnitOfWork.CommitAsync(); // these are equal as the data service method calls the UoF method underneath
            //await base.CommitAsync();
            
            return mapped;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding book {@Book}", request);
            return ex;
        }
    }
}
