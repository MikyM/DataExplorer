using BookLibrary.Application.Models;
using BookLibrary.DataAccessLayer;
using BookLibrary.DataAccessLayer.Specifications;
using BookLibrary.Domain;
using DataExplorer.EfCore.Abstractions.DataServices;
using DataExplorer.EfCore.Specifications;
using Gridify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/borrowing")]
public class BorrowingController(ILogger<BorrowingController> logger, ICrudDataService<Borrowing, ILibraryDbContext> dataService, TimeProvider timeProvider) : ControllerBase
{
    [HttpPost]
    [ActionName("AddBorrowing")]
    public async Task<IActionResult> AddBorrowingAsync([FromBody] AddBorrowingRequest borrowing)
    {
        var mapped = dataService.Mapper.Map<Borrowing>(borrowing);

        mapped.BorrowedAt = timeProvider.GetUtcNow();
        
        var borrowingsResult = await dataService.AddAsync(mapped, true);
        
        if (borrowingsResult.IsDefined(out var id))
        {
            logger.LogInformation("Added new borrowing {@Borrowing}", mapped);
            
            const string actionName = "GetBorrowingById";
            var routeValues =  new { id };
            
            return CreatedAtAction(actionName, routeValues, mapped);
        }

        return Problem();
    }
    
    [HttpPatch("{id:long}/return")]
    public async Task<IActionResult> ReturnBorrowingAsync(long id)
    {
        var borrowingsResult = await dataService.ExecuteUpdateAsync(new ReturnBorrowingSpec(id, timeProvider.GetUtcNow()));
        
        if (borrowingsResult.IsDefined(out var cnt))
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
    public async Task<IActionResult> DeleteBorrowingAsync(long id)
    {
        var borrowingsResult = await dataService.ExecuteUpdateAsync(new DisableSpecification<Borrowing>(id));
        
        if (borrowingsResult.IsDefined(out var cnt))
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
    [ActionName("GetBorrowingById")]
    public async Task<IActionResult> GetBorrowingAsync(long id)
    {
        var borrowingsResult = await dataService.GetSingleAsync(new BorrowingWithInfoSpec(id));
        
        return borrowingsResult.IsDefined(out var borrowing) 
            ? Ok(borrowing) 
            : NotFound();
    }
    
    
    [HttpGet("query")]
    public async Task<IActionResult> GetAllBorrowingsAsync([FromQuery] GridifyQuery query)
    {
        var borrowings = await dataService.GetByGridifyQueryAsync(query);
        
        return Ok(borrowings);
    }
}
