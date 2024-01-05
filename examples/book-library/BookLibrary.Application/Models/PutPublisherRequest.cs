using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Application.Models;

public class PutPublisherRequest
{
    [Required]
    public long Id { get; set; }
    [Required]
    public string? Name { get; set; }
}
