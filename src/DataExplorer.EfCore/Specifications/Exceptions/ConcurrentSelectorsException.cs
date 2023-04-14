namespace DataExplorer.EfCore.Specifications.Exceptions;

/// <summary>
/// An exception that occurs when there is a concurrent specification selector transforms defined.
/// </summary>
[PublicAPI]
public class ConcurrentSelectorsException : Exception
{
    private const string ConstMessage = "Concurrent specification selector transforms defined. Ensure only one of the Select() or SelectMany() transforms is used in the same specification!";

    public ConcurrentSelectorsException()
        : base(ConstMessage)
    {
    }

    public ConcurrentSelectorsException(Exception innerException)
        : base(ConstMessage, innerException)
    {
    }
}
