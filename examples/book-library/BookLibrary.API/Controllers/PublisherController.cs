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
[Route("api/publisher")]
public class PublisherController(ILogger<PublisherController> logger, ICrudDataService<Publisher, ILibraryDbContext> dataService) : ControllerBase
{
    [HttpPost]
    [ActionName("AddPublisher")]
    public async Task<IActionResult> AddPublisherAsync([FromBody] AddPublisherRequest publisher)
    {
        var mapped = dataService.Mapper.Map<Publisher>(publisher);
        
        var publishersResult = await dataService.AddAsync(mapped, true);
        
        if (publishersResult.IsDefined(out var id))
        {
            logger.LogInformation("Added new publisher {@Publisher}", mapped);
            
            const string actionName = "GetPublisherById";
            var routeValues =  new { id };
            
            return CreatedAtAction(actionName, routeValues, mapped);
        }

        return Problem();
    }
    
    [HttpPut]
    public async Task<IActionResult> PutPublisherAsync([FromBody] PutPublisherRequest publisher)
    {
        var mapped = dataService.Mapper.Map<Publisher>(publisher);
        
        var publishersResult = await dataService.ExecuteUpdateAsync(new PutPublisherSpec(publisher.Id, mapped));
        
        if (publishersResult.IsDefined(out var cnt))
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
    public async Task<IActionResult> DeletePublisherAsync(long id)
    {
        var publishersResult = await dataService.ExecuteUpdateAsync(new DisableSpecification<Publisher>(id));
        
        if (publishersResult.IsDefined(out var cnt))
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
    [ActionName("GetPublisherById")]
    public async Task<IActionResult> GetPublisherAsync(long id)
    {
        var publishersResult = await dataService.GetSingleAsync(new PublisherWithBooksSpec(id));
        
        return publishersResult.IsDefined(out var publisher) 
            ? Ok(publisher) 
            : NotFound();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPublisherAsync([FromQuery] string name)
    {
        var publishersResult = await dataService.GetAsync(new PublisherWithBooksSpec(name));
        
        return publishersResult.IsDefined(out var publisher) 
            ? Ok(publisher) 
            : Problem();
    }
    
    [HttpGet("query")]
    public async Task<IActionResult> GetAllPublishersAsync([FromQuery] GridifyQuery query)
    {
        var publishers = await dataService.GetByGridifyQueryAsync(query);
        
        return Ok(publishers);
    }
}
