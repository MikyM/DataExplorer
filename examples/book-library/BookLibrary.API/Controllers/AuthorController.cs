﻿using BookLibrary.Application.Models;
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
[Route("api/author")]
public class AuthorController(ILogger<AuthorController> logger, ICrudDataService<Author, ILibraryDbContext> dataService) : ControllerBase
{
    [HttpPost]
    [ActionName("AddAuthor")]
    public async Task<IActionResult> AddAuthorAsync([FromBody] AddAuthorRequest author)
    {
        var mapped = dataService.Mapper.Map<Author>(author);
        
        var authorsResult = await dataService.AddAsync(mapped, true);
        
        if (authorsResult.IsDefined(out var id))
        {
            logger.LogInformation("Added new author {@Author}", mapped);
            
            var actionName = "GetAuthorById";
            var routeValues = new { id = mapped.Id };
            
            return CreatedAtAction(actionName, routeValues, mapped);
        }

        return Problem();
    }
    
    [HttpPut]
    public async Task<IActionResult> PutAuthorAsync([FromBody] PutAuthorRequest author)
    {
        var mapped = dataService.Mapper.Map<Author>(author);
        
        var authorsResult = await dataService.ExecuteUpdateAsync(new PutAuthorSpec(author.Id, mapped));
        
        if (authorsResult.IsDefined(out var cnt))
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
    public async Task<IActionResult> DeleteAuthorAsync(long id)
    {
        var authorsResult = await dataService.ExecuteUpdateAsync(new DisableSpecification<Author>(id));
        
        if (authorsResult.IsDefined(out var cnt))
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
    [ActionName("GetAuthorById")]
    public async Task<IActionResult> GetAuthorAsync(long id)
    {
        var authorsResult = await dataService.GetSingleAsync(new AuthorWithBooksSpec(id));
        
        return authorsResult.IsDefined(out var author) 
            ? Ok(author) 
            : NotFound();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAuthorAsync([FromQuery] string surname, [FromQuery] string? firstName)
    {
        var authorsResult = await dataService.GetSingleAsync(new AuthorWithBooksSpec(surname, firstName));
        
        return authorsResult.IsDefined(out var author) 
            ? Ok(author) 
            : Problem();
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAuthorsAsync([FromQuery] GridifyQuery query)
    {
        var authors = await dataService.GetByGridifyQueryAsync(query);
        
        return Ok(authors);
    }
}
