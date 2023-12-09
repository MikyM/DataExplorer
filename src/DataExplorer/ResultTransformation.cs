namespace DataExplorer;

/// <summary>
/// Defines result transformation type.
/// </summary>
[PublicAPI]
public enum ResultTransformation
{
    /// <summary>
    /// Makes the query use AutoMapper's ProjectTo method.
    /// </summary>
    ProjectTo,
    /// <summary>
    /// Maps the result after obtaining them using AutoMapper's Map method.
    /// </summary>
    Map
}
