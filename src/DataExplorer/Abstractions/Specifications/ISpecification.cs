using System.Linq.Expressions;
using AutoMapper;
using DataExplorer.Specifications;
using DataExplorer.Specifications.Expressions;

namespace DataExplorer.Abstractions.Specifications;

/// <summary>
///     Encapsulates query logic for <typeparamref name="T" />,
///     and projects the result into <typeparamref name="TResult" />.
/// </summary>
/// <typeparam name="T">The type being queried against.</typeparam>
/// <typeparam name="TResult">The type of the result to project results to with Automapper's ProjectTo.</typeparam>
[PublicAPI]
public interface ISpecification<T, TResult> : ISpecification<T> where T : class
{
    /// <summary>
    /// Automapper configuration
    /// </summary>
    IConfigurationProvider? MapperConfigurationProvider { get; set; }

    /// <summary>
    /// Members to expand
    /// </summary>
    IEnumerable<Expression<Func<TResult, object>>>? MembersToExpand { get; set; }

    /// <summary>
    /// Members to expand
    /// </summary>
    IEnumerable<string>? StringMembersToExpand { get; set; }

    /// <summary>
    ///     The transform function to apply to the result of the query encapsulated by the
    ///     <see cref="ISpecification{T, TResult}" />.
    /// </summary>
    new Func<IEnumerable<TResult>, IEnumerable<TResult>>? PostProcessingAction { get; set; }

    /// <summary>
    /// Evaluates the given set of entities
    /// </summary>
    /// <param name="entities">Entities to evaluate</param>
    /// <returns></returns>
    new IEnumerable<TResult> Evaluate(IEnumerable<T> entities);

    /// <summary>
    /// The transform function to apply to the <typeparamref name="T"/> element.
    /// </summary>
    Expression<Func<T, TResult>>? Selector { get; set; }
    
    /// <summary>
    /// The SelectMany transform function to apply to the <typeparamref name="T"/> element.
    /// </summary>
    Expression<Func<T, IEnumerable<TResult>>>? SelectorMany { get; set; }
}

/// <summary>
///     Encapsulates extended query logic for <typeparamref name="T" />.
/// </summary>
/// <typeparam name="T">The type being queried against.</typeparam>
public interface ISpecification<T> : IBasicSpecification<T> where T : class
{
    /// <summary>
    /// The collection of <see cref="IncludeExpressionInfo"/>s describing each include expression.
    /// This information is utilized to build Include/ThenInclude functions in the query.
    /// </summary>
    IEnumerable<IncludeExpressionInfo>? IncludeExpressions { get; set; }

    /// <summary>
    ///     The collection of navigation properties, as strings, to include in the query.
    /// </summary>
    IEnumerable<string>? IncludeStrings { get; set; }
    
    /// <summary>
    ///     Cache timeout if any.
    /// </summary>
    TimeSpan? CacheTimeout { get; set; }

    /// <summary>
    ///     Cache expiration mode if any.
    /// </summary>
    CacheExpirationMode? CacheExpirationMode { get; set; }
    
    /// <summary>
    ///     The transform function to apply to the result of the query encapsulated by the <see cref="ISpecification{T}" />.
    /// </summary>
    Func<IEnumerable<T>, IEnumerable<T>>? PostProcessingAction { get; set; }

    /// <summary>
    ///     Whether or not the results should be cached, setting this will override default caching settings for this query.
    ///     Defaults to null - no override.
    /// </summary>
    bool? IsCacheEnabled { get; set; }

    /// <summary>
    ///     Returns whether or not the change tracker will track any of the entities
    ///     that are returned. When true, if the entity instances are modified, this will not be detected
    ///     by the change tracker.
    /// </summary>
    bool IsAsNoTracking { get; set; }
    
    /// <summary>
    ///     Returns whether or not the change tracker will track any of the entities
    ///     that are returned.
    /// </summary>
    bool IsAsTracking { get; set; }
    
    /// <summary>
    ///     Returns whether or not to treat this query as split query.
    ///     by the change tracker.
    /// </summary>
    /// 
    bool IsAsSplitQuery { get; set; }

    /// <summary>
    ///     Returns whether or not the change tracker with track the result of this query identity resolution.
    /// </summary>
    bool IsAsNoTrackingWithIdentityResolution { get; set; }
}

/// <summary>
///     Encapsulates base query logic for <typeparamref name="T" />.
/// </summary>
[PublicAPI]
public interface IBasicSpecification<T> : ISpecification where T : class
{
    /// <summary>
    ///     Pagination filter to apply.
    /// </summary>
    PaginationFilter? PaginationFilter { get; set; }

    /// <summary>
    /// Whether pagination is enabled
    /// </summary>
    bool IsPagingEnabled { get; set; }
    
    /// <summary>
    ///     The number of elements to return.
    /// </summary>
    int? Take { get; set; }

    /// <summary>
    ///     The number of elements to skip before returning the remaining elements.
    /// </summary>
    int? Skip { get; set; }
    
    /// <summary>
    ///     The collection of predicates to group by.
    /// </summary>
    Expression<Func<T, object>>? GroupByExpression { get; set; }

    /// <summary>
    /// The collections of functions used to determine the sorting (and subsequent sorting),
    /// to apply to the result of the query encapsulated by the <see cref="ISpecification{T}"/>.
    /// </summary>
    IEnumerable<OrderExpressionInfo<T>>? OrderExpressions { get; set; }
    
    /// <summary>
    /// Returns whether or not the query should ignore the defined global query filters 
    /// </summary>
    /// <remarks>
    /// for more info: https://docs.microsoft.com/en-us/ef/core/querying/filters
    /// </remarks>
    bool IgnoreQueryFilters { get; set; }

    /// <summary>
    /// It returns whether the given entity satisfies the conditions of the specification.
    /// </summary>
    /// <param name="entity">The entity to be validated</param>
    /// <returns></returns>
    bool IsSatisfiedBy(T entity);

    /// <summary>
    /// Evaluates the given set of entities
    /// </summary>
    /// <param name="entities">Entities to evaluate</param>
    /// <returns></returns>
    IEnumerable<T> Evaluate(IEnumerable<T> entities);
    
    /// <summary>
    /// The collection of 'SQL LIKE' operations.
    /// </summary>
    IEnumerable<SearchExpressionInfo<T>>? SearchCriterias { get; set; }
    
    /// <summary>
    /// The collection of filters.
    /// </summary>
    IEnumerable<WhereExpressionInfo<T>>? WhereExpressions { get; set; }
}

/// <summary>
/// Marker specification interface
/// </summary>
public interface ISpecification
{
}
