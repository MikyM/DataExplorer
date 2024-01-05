using BookLibrary.Application.Models;
using BookLibrary.DataAccessLayer;
using BookLibrary.Domain;
using DataExplorer.EfCore.Abstractions.DataServices;
using Remora.Results;

namespace BookLibrary.Application.Services.DataServices.Abstractions;

public interface IBookDataService : ICrudDataService<Book, ILibraryDbContext>
{
    Task<Result<Book>> AddBookAsync(AddBookRequest request);
}
