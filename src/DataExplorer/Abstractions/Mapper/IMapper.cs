namespace DataExplorer.Abstractions.Mapper;

/// <summary>
/// Represents a mapper used by DataExplorer services.
/// </summary>
public interface IMapper
{
    /// <summary>
    /// Maps given object to another type.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <typeparam name="TTarget">The target type.</typeparam>
    /// <returns>An instance of a target created based on the source object.</returns>
    TTarget Map<TTarget>(object source);

    /// <summary>
    /// Projects a queryable to another type.
    /// </summary>
    /// <param name="source">The source queryable.</param>
    /// <param name="parameters">Additional parameters.</param>
    /// <param name="membersToExpand">Members to expand.</param>
    /// <typeparam name="TTarget">The target type.</typeparam>
    /// <returns>A queryable of the target type.</returns>
    IQueryable<TTarget> ProjectTo<TTarget>(IQueryable source, IDictionary<string,object> parameters, params string[] membersToExpand);
}