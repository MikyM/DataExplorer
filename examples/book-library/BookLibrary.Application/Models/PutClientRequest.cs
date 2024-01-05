namespace BookLibrary.Application.Models;

public record PutClientRequest( long Id, string? FirstName, string? Surname, string? PhoneNumber, string? Email);
