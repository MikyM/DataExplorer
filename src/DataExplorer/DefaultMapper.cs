using DataExplorer.Abstractions.Mapper;

namespace DataExplorer;

/// <inheritdoc/>
[PublicAPI]
public sealed class DefaultMapper : IMapper
{
    /// <inheritdoc/>
    public TTarget Map<TTarget>(object source)
    {
        throw new NotImplementedException("You must implement your mapper or use a NuGet that provides one in order to use mapping");
    }

    /// <inheritdoc/>
    public IQueryable<TTarget> ProjectTo<TTarget>(IQueryable source, IDictionary<string, object> parameters, params string[] membersToExpand)
    {
        throw new NotImplementedException("You must implement your mapper or use a NuGet that provides one in order to use mapping");
    }
}