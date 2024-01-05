using BookLibrary.Application.Models;
using BookLibrary.DataAccessLayer;
using BookLibrary.Domain;
using DataExplorer.EfCore.Abstractions.DataServices;
using Remora.Results;

namespace BookLibrary.Application.Services.DataServices.Abstractions;

public interface IBorrowingDataService : ICrudDataService<Borrowing, ILibraryDbContext>
{
    Task<Result<Borrowing>> AddBorrowingAsync(AddBorrowingRequest request);
}
