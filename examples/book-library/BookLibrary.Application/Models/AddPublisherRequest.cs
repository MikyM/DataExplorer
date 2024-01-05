using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Application.Models;

public class AddPublisherRequest
{
    [Required]
    public string? Name { get; set; }
}
