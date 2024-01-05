using BookLibrary.Application.Models;
using BookLibrary.Application.Services.DataServices.Abstractions;
using BookLibrary.DataAccessLayer;
using BookLibrary.Domain;
using DataExplorer.EfCore.Abstractions;
using Remora.Results;

namespace BookLibrary.Application.Services.DataServices;

[UsedImplicitly]
public class BorrowingDataService : CrudDataService<Borrowing, ILibraryDbContext>, IBorrowingDataService
{
    private readonly TimeProvider _timeProvider;
    
    public BorrowingDataService(IUnitOfWork<ILibraryDbContext> uof, TimeProvider timeProvider) : base(uof)
    {
        _timeProvider = timeProvider;
    }

    public async Task<Result<Borrowing>> AddBorrowingAsync(AddBorrowingRequest request)
    {
        var mapped = Mapper.Map<Borrowing>(request);

        mapped.BorrowedAt = _timeProvider.GetUtcNow();
        
        var addRes = await base.AddAsync(mapped, true);
        
        return addRes.IsSuccess 
            ? mapped 
            : Result<Borrowing>.FromError(addRes);
    }
}
