using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Application.Models;

public class AddAuthorRequest
{
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? Surname { get; set; }
}
