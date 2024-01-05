namespace BookLibrary.Application.Models;

public record AddBookRequest( string? Title, DateTimeOffset PublishedAt, long AuthorId, long PublisherId);
