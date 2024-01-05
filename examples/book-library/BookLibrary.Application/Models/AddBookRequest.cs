using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Application.Models;

public class AddBookRequest
{
    [Required]
    public string? Title { get; set; }
    [Required]
    public DateTimeOffset PublishedAt { get; set; }
    [Required]
    public long AuthorId { get; set; }
    [Required]
    public long PublisherId { get; set; }
}
