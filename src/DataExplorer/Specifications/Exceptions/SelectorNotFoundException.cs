namespace DataExplorer.Specifications.Exceptions;

/// <summary>
/// An exception that occurs when a selector is not found.
/// </summary>
[PublicAPI]
public class SelectorNotFoundException : Exception
{
    private const string ConstMessage = "The specification must have a selector transform defined. Ensure either Select() or SelectMany() is used in the specification!";

    public SelectorNotFoundException()
        : base(ConstMessage)
    {
    }

    public SelectorNotFoundException(Exception innerException)
        : base(ConstMessage, innerException)
    {
    }
}
