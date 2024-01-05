namespace BookLibrary.Application.Models;

public record PutBookRequest( long Id, string? Title, DateTimeOffset PublishedAt, long AuthorId, long PublisherId);
