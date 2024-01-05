using BookLibrary.DataAccessLayer;
using DataExplorer.EfCore.Abstractions.DataServices;
using DataExplorer.EfCore.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/client")]
public class ClientController(ILogger<ClientController> logger, ICrudDataService<Client, ILibraryDbContext> dataService) : ControllerBase
{
    [HttpPost]
    [ActionName("AddClient")]
    public async Task<IActionResult> AddClientAsync([FromBody] AddClientRequest request)
    {
        var mapped = dataService.Mapper.Map<Client>(request);
        
        var clientsResult = await dataService.AddAsync(mapped, true);
        
        if (clientsResult.IsDefined(out var id))
        {
            logger.LogInformation("Added new client {@Client}", mapped);
            
            const string actionName = "GetClientById";
            var routeValues =  new { id };
            
            return CreatedAtAction(actionName, routeValues, mapped);
        }

        return Problem();
    }
    
    [HttpPut]
    public async Task<IActionResult> PutClientAsync([FromBody] PutClientRequest client)
    {
        var mapped = dataService.Mapper.Map<Client>(client);
        
        var clientsResult = await dataService.ExecuteUpdateAsync(new PutClientSpec(client.Id, mapped));
        
        if (clientsResult.IsDefined(out var cnt))
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
    public async Task<IActionResult> DeleteClientAsync(long id)
    {
        var clientsResult = await dataService.ExecuteUpdateAsync(new DisableSpecification<Client>(id));
        
        if (clientsResult.IsDefined(out var cnt))
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
    [ActionName("GetClientById")]
    public async Task<IActionResult> GetClientAsync(long id)
    {
        var clientsResult = await dataService.GetSingleAsync(new ClientWithBorrowingsSpec(id));
        
        return clientsResult.IsDefined(out var client) 
            ? Ok(client) 
            : NotFound();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetClientAsync([FromQuery] string surname, [FromQuery] string? firstName)
    {
        var clientsResult = await dataService.GetAsync(new ClientWithBorrowingsSpec(firstName, surname));
        
        return clientsResult.IsDefined(out var client) 
            ? Ok(client) 
            : Problem();
    }
    
    [HttpGet("query")]
    public async Task<IActionResult> GetAllClientsAsync([FromQuery] GridifyQuery query)
    {
        var clients = await dataService.GetByGridifyQueryAsync(query);
        
        return clients.IsDefined(out var paging) 
            ? Ok(paging) 
            : Problem();
    }
}
