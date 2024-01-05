using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Application.Models;

public class PutAuthorRequest
{
    [Required]
    public long Id { get; set; }
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? Surname { get; set; }
}
