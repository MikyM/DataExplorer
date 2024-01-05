using BookLibrary.Application.Services.DataServices.Abstractions;
using BookLibrary.DataAccessLayer;
using DataExplorer.EfCore.Abstractions.DataServices;
using DataExplorer.EfCore.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/book")]
public class BookController(ILogger<BookController> logger, IBookDataService dataService) : ControllerBase
{
    [HttpPost]
    [ActionName("AddBook")]
    public async Task<IActionResult> AddBookAsync([FromBody] AddBookRequest request)
    {
        var booksResult = await dataService.AddBookAsync(request);
        
        if (booksResult.IsDefined(out var book))
        {
            logger.LogInformation("Added new book {@Book}", book);
            
            const string actionName = "GetBookById";
            var routeValues =  new { id = book.Id };
            
            return CreatedAtAction(actionName, routeValues, book);
        }

        return Problem();
    }
    
    [HttpPut]
    public async Task<IActionResult> PutBookAsync([FromBody] PutBookRequest book)
    {
        var mapped = dataService.Mapper.Map<Book>(book);
        
        var booksResult = await dataService.ExecuteUpdateAsync(new PutBookSpec(book.Id, mapped));
        
        if (booksResult.IsDefined(out var cnt))
        {
            if (cnt >= 1)
            {
                return NoContent();
            }
            
            return NotFound();
        }

        return Problem();
    }
    
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteBookAsync(long id)
    {
        var booksResult = await dataService.ExecuteUpdateAsync(new DisableSpecification<Book>(id));
        
        if (booksResult.IsDefined(out var cnt))
        {
            if (cnt >= 1)
            {
                return NoContent();
            }
            
            return NotFound();
        }

        return Problem();
    }
    
    [HttpGet("{id:long}")]
    [ActionName("GetBookById")]
    public async Task<IActionResult> GetBookAsync(long id)
    {
        var booksResult = await dataService.GetSingleAsync(new BookWithInfoSpec(id));
        
        return booksResult.IsDefined(out var book) 
            ? Ok(book) 
            : NotFound();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBookAsync([FromQuery] string title)
    {
        var booksResult = await dataService.GetAsync(new BookWithInfoSpec(title));
        
        return booksResult.IsDefined(out var book) 
            ? Ok(book) 
            : Problem();
    }
    
    [HttpGet("query")]
    public async Task<IActionResult> GetAllBooksAsync([FromQuery] GridifyQuery query)
    {
        var books = await dataService.GetByGridifyQueryAsync(query);
        
        return books.IsDefined(out var paging) 
            ? Ok(paging) 
            : Problem();
    }
}
