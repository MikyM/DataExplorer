namespace DataExplorer.Abstractions;

/// <summary>
/// Represents a DI resolver.
/// </summary>
[PublicAPI]
public interface IResolver
{
    T Resolve<T>();
    T ResolveKeyed<T>(object key);
}