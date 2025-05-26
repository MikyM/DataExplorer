using System.Linq.Expressions;
using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Builders;
using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.Abstractions.Specifications.Validators;
using DataExplorer.EfCore.Specifications.Builders;
using DataExplorer.EfCore.Specifications.Evaluators;
using DataExplorer.EfCore.Specifications.Validators;
using DataExplorer.Specifications.Expressions;
using EFCoreSecondLevelCacheInterceptor;
using CacheExpirationMode = DataExplorer.Specifications.CacheExpirationMode;

namespace DataExplorer.EfCore.Specifications;

/// <inheritdoc cref="ISpecification" />
[PublicAPI]
public class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult> where T : class
{
    protected internal Specification() : this(InMemorySpecificationEvaluator.Default)
    {
    }

    public Specification(PaginationFilter paginationFilter) : this(InMemorySpecificationEvaluator.Default, paginationFilter)
    {
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator, PaginationFilter paginationFilter) : base(
        inMemorySpecificationEvaluator, paginationFilter)
    {
        Query = new SpecificationBuilder<T, TResult>(this);
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator) : base(
        inMemorySpecificationEvaluator)
    {
        Query = new SpecificationBuilder<T, TResult>(this);
    }

    /// <summary>
    /// Inner <see cref="ISpecificationBuilder{T,TResult}"/>
    /// </summary>
    protected new ISpecificationBuilder<T, TResult> Query { get; }

    /// <inheritdoc />
    public new virtual IEnumerable<TResult> Evaluate(IEnumerable<T> entities) 
        => Evaluator.Evaluate(entities, this);

    /// <inheritdoc/>
    public Expression<Func<T, TResult>>? Selector { get; set; }
    
    /// <inheritdoc/>
    public Expression<Func<T, IEnumerable<TResult>>>? SelectorMany { get; set; }

    /// <inheritdoc />
    public IEnumerable<Expression<Func<TResult, object>>>? MembersToExpand { get; set; }

    /// <inheritdoc />
    public IEnumerable<string>? StringMembersToExpand { get; set; }

    /// <inheritdoc />
    public new Func<IEnumerable<TResult>, IEnumerable<TResult>>? PostProcessingAction { get; set; }

    /// <summary>
    /// Specify a transform function to apply to the result of the query.
    /// and returns another <typeparamref name="TResult"/> type
    /// </summary>
    protected ISpecificationBuilder<T,TResult> WithPostProcessingAction(Func<IEnumerable<TResult>, IEnumerable<TResult>> postProcessingAction) 
        => Query.WithPostProcessingAction(postProcessingAction);

    /// <summary>
    /// Specify a transform function to apply to the <typeparamref name="T"/> element 
    /// </summary>
    /// <param name="selector">Selector</param>
    /// <returns>Current specification instance</returns>
    protected ISpecificationBuilder<T,TResult> Select(Expression<Func<T, TResult>> selector) => 
        Query.Select(selector);

    /// <summary>
    /// Specify a transform function to apply to the <typeparamref name="T"/> element 
    /// to produce a flattened sequence of <typeparamref name="TResult"/> elements.
    /// </summary>
    protected ISpecificationBuilder<T,TResult> SelectMany(Expression<Func<T, IEnumerable<TResult>>> selector) 
        => Query.SelectMany(selector);

    /// <summary>
    /// Expands given member.
    /// </summary>
    /// <param name="expression">Member to expand</param>
    /// <returns>Current specification instance</returns>
    protected ISpecificationBuilder<T,TResult> Expand(Expression<Func<TResult, object>> expression) 
        => Query.Expand(expression);

    /// <summary>
    /// Expands given member.
    /// </summary>
    /// <param name="member">Member to expand</param>
    /// <returns>Current specification instance</returns>
    protected ISpecificationBuilder<T,TResult> Expand(string member) 
        => Query.Expand(member);
}

/// <inheritdoc cref="ISpecification{T}" />
[PublicAPI]
public class Specification<T> : BasicSpecification<T>,  ISpecification<T> where T : class
{
    protected Specification(PaginationFilter paginationFilter)
        : this(InMemorySpecificationEvaluator.Default, SpecificationValidator.Default, paginationFilter)
    {
    }

    protected Specification()
        : this(InMemorySpecificationEvaluator.Default, SpecificationValidator.Default)
    {
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator)
        : this(inMemorySpecificationEvaluator, SpecificationValidator.Default)
    {
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator, PaginationFilter paginationFilter)
        : this(inMemorySpecificationEvaluator, SpecificationValidator.Default, paginationFilter)
    {
    }

    protected Specification(ISpecificationValidator specificationValidator, PaginationFilter paginationFilter)
        : this(InMemorySpecificationEvaluator.Default, specificationValidator, paginationFilter)
    {
    }

    protected Specification(ISpecificationValidator specificationValidator)
        : this(InMemorySpecificationEvaluator.Default, specificationValidator)
    {
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator, ISpecificationValidator specificationValidator): base(
        inMemorySpecificationEvaluator, specificationValidator)
    {
        Query = new SpecificationBuilder<T>(this);
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator,
        ISpecificationValidator specificationValidator, PaginationFilter paginationFilter) : base(
        inMemorySpecificationEvaluator, specificationValidator)
    {
        PaginationFilter = paginationFilter;
        Query = new SpecificationBuilder<T>(this);
    }
    
    /// <inheritdoc />
    public IEnumerable<IncludeExpressionInfo>? IncludeExpressions { get; set; }
    
    /// <inheritdoc />
    public IEnumerable<string>? IncludeStrings { get; set; }

    /// <inheritdoc />
    public TimeSpan? CacheTimeout { get; set; }

    /// <inheritdoc />
    public CacheExpirationMode? CacheExpirationMode { get; set; }

    /// <inheritdoc />
    public Func<IEnumerable<T>, IEnumerable<T>>? PostProcessingAction { get; set; }

    /// <inheritdoc />
    public bool? IsCacheEnabled { get; set; }
    
    /// <inheritdoc />
    public bool IsAsNoTracking { get; set; } = true;

    /// <inheritdoc />
    public bool IsAsSplitQuery { get; set; }

    /// <inheritdoc />
    public bool IsAsNoTrackingWithIdentityResolution { get; set; }

    /// <inheritdoc />
    public bool IsAsTracking { get; set; }
    
    /// <summary>
    /// Inner <see cref="ISpecificationBuilder{T,TResult}"/>
    /// </summary>
    protected new ISpecificationBuilder<T> Query { get; }

    /// <summary>
    /// The query will ignore the defined global query filters
    /// </summary>
    /// <remarks>
    /// for more info: https://docs.microsoft.com/en-us/ef/core/querying/filters
    /// </remarks>
    protected ISpecificationBuilder<T> WithIgnoreQueryFilters()
        => WithIgnoreQueryFilters(true);
    
    /// <summary>
    /// The query will ignore the defined global query filters
    /// </summary>
    /// <remarks>
    /// for more info: https://docs.microsoft.com/en-us/ef/core/querying/filters
    /// </remarks>
    protected ISpecificationBuilder<T> WithIgnoreQueryFilters(bool condition)
        => Query.IgnoreQueryFilters(condition);

    /// <summary>
    /// Specify a transform function to apply to the result of the query 
    /// and returns the same <typeparamref name="T"/> type
    /// </summary>
    /// <param name="postProcessingAction">Action to use for post processing</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> WithPostProcessingAction(Func<IEnumerable<T>, IEnumerable<T>> postProcessingAction)
        => Query.WithPostProcessingAction(postProcessingAction);

    /// <summary>
    /// Specify whether results should be cached using <see cref="SecondLevelCacheInterceptor"/>.
    /// </summary>
    /// <param name="withCaching">Whether to cache results</param>
    /// <returns><see cref="ICacheSpecificationBuilder{T}"/> instance</returns>
    protected ICacheSpecificationBuilder<T> WithCaching(bool withCaching = true)
        => Query.WithCaching(withCaching);

    /// <summary>
    /// If the entity instances are modified, this will be detected
    /// by the change tracker.
    /// </summary>
    protected ISpecificationBuilder<T> AsTracking()
        => AsTracking(true);
    
    /// <summary>
    /// If the entity instances are modified, this will be detected
    /// by the change tracker.
    /// </summary>
    /// <param name="condition">If false, the setting will be discarded.</param>
    protected ISpecificationBuilder<T> AsTracking(bool condition)
        => Query.AsTracking(condition);
    
    /// <summary>
    /// The query will then keep track of returned instances 
    /// (without tracking them in the normal way) 
    /// and ensure no duplicates are created in the query results
    /// </summary>
    /// <remarks>
    /// for more info: https://docs.microsoft.com/en-us/ef/core/change-tracking/identity-resolution#identity-resolution-and-queries
    /// </remarks>
    protected ISpecificationBuilder<T> AsNoTrackingWithIdentityResolution()
        => AsNoTrackingWithIdentityResolution(true);
    
    /// <summary>
    /// The query will then keep track of returned instances 
    /// (without tracking them in the normal way) 
    /// and ensure no duplicates are created in the query results
    /// </summary>
    /// <remarks>
    /// for more info: https://docs.microsoft.com/en-us/ef/core/change-tracking/identity-resolution#identity-resolution-and-queries
    /// </remarks>
    /// <param name="condition">If false, the setting will be discarded.</param>
    protected ISpecificationBuilder<T> AsNoTrackingWithIdentityResolution(bool condition)
        => Query.AsNoTrackingWithIdentityResolution(condition);

    /// <summary>
    /// Specify whether to use tracking use split query
    /// </summary>
    /// <returns><see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> AsSplitQuery()
        => Query.AsSplitQuery();

    /// <summary>
    /// If the entity instances are modified, this will not be detected
    /// by the change tracker.
    /// </summary>
    protected ISpecificationBuilder<T> AsNoTracking()
        => AsNoTracking(true);
    
    /// <summary>
    /// If the entity instances are modified, this will not be detected
    /// by the change tracker.
    /// </summary>
    /// <param name="condition">If false, the setting will be discarded.</param>
    protected ISpecificationBuilder<T> AsNoTracking(bool condition)
        => Query.AsNoTracking(condition);
    
    /// <summary>
    /// Specify an include expression.
    /// This information is utilized to build Include function in the query, which ORM tools like Entity Framework use
    /// to include related entities (via navigation properties) in the query result.
    /// </summary>
    /// <param name="includeExpression">Member to include</param>
    /// <returns><see cref="IIncludableSpecificationBuilder{T,TProperty}"/> instance</returns>
    protected IIncludableSpecificationBuilder<T, TProperty?> Include<TProperty>(
        Expression<Func<T, TProperty?>> includeExpression)
        => Query.Include(includeExpression);
    
    /// <summary>
    /// Specify a predicate that will be applied to the query
    /// </summary>
    /// <param name="criteria">Given criteria</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected sealed override ISpecificationBuilder<T> Where(Expression<Func<T, bool>> criteria)
        => Query.Where(criteria);
    
    /// <summary>
    /// Specify a 'SQL LIKE' operations for search purposes
    /// </summary>
    /// <param name="selector">the property to apply the SQL LIKE against</param>
    /// <param name="searchTerm">the value to use for the SQL LIKE</param>
    /// <param name="searchGroup">the index used to group sets of Selectors and SearchTerms together</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected sealed override ISpecificationBuilder<T> Search(Expression<Func<T, string>> selector, string searchTerm, int searchGroup = 1)
        => Query.Search(selector, searchTerm, searchGroup);
    
    /// <summary>
    /// Specify a <see cref="PaginationFilter"/> to use
    /// </summary>
    /// <param name="paginationFilter"><see cref="PaginationFilter"/> to use</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected sealed override ISpecificationBuilder<T> WithPaginationFilter(PaginationFilter paginationFilter)
        => Query.WithPaginationFilter(paginationFilter);

    /// <summary>
    /// Specify the number of elements to return.
    /// </summary>
    /// <param name="take">number of elements to take</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected sealed override ISpecificationBuilder<T> ApplyTake(int take)
        => Query.Take(take);

    /// <summary>
    /// Specify the number of elements to skip before returning the remaining elements.
    /// </summary>
    /// <param name="skip">number of elements to skip</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected sealed override ISpecificationBuilder<T> ApplySkip(int skip)
        => Query.Skip(skip);
    
    /// <summary>
    /// Specify the query result will be grouped by <paramref name="groupByExpression"/> in a descending order
    /// </summary>
    /// <param name="groupByExpression">Member to use for grouping</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected sealed override IGroupedSpecificationBuilder<T> GroupBy(Expression<Func<T, object>> groupByExpression)
        => Query.GroupBy(groupByExpression);
    
    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderByExpression"/> in an ascending order
    /// </summary>
    /// <param name="orderByExpression">Member to use for ordering</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected sealed override IOrderedSpecificationBuilder<T> OrderBy(Expression<Func<T, object?>> orderByExpression)
        => Query.OrderBy(orderByExpression);

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderByDescendingExpression"/> in a descending order
    /// </summary>
    /// <param name="orderByDescendingExpression">Member to use for ordering</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected sealed override IOrderedSpecificationBuilder<T> OrderByDescending(Expression<Func<T, object?>> orderByDescendingExpression)
        => Query.OrderByDescending(orderByDescendingExpression);
}

/// <inheritdoc cref="ISpecification{T}" />
[PublicAPI]
public class BasicSpecification<T> : IBasicSpecification<T> where T : class
{
    internal BasicSpecification()
        : this(InMemorySpecificationEvaluator.Default, SpecificationValidator.Default)
    {
    }

    internal BasicSpecification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator)
        : this(inMemorySpecificationEvaluator, SpecificationValidator.Default)
    {
    }

    internal BasicSpecification(ISpecificationValidator specificationValidator)
        : this(InMemorySpecificationEvaluator.Default, specificationValidator)
    {
    }

    internal BasicSpecification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator, ISpecificationValidator specificationValidator)
    {
        Evaluator = inMemorySpecificationEvaluator;
        Validator = specificationValidator;
        Query = new BasicSpecificationBuilder<T>(this);
    }
    
    private PaginationFilter? _paginationFilter;

    /// <inheritdoc />
    public PaginationFilter? PaginationFilter
    {
        get
        {
            if (_paginationFilter is not null) return _paginationFilter;
            if (!Take.HasValue || !Skip.HasValue) return null;

            _paginationFilter = new PaginationFilter(Skip.Value / Take.Value + 1, Take.Value);
            if (!IsPagingEnabled) IsPagingEnabled = true;

            return _paginationFilter;

        }
        set
        {
            if (value is null)
            {
                _paginationFilter = value;
                return;
            }

            _paginationFilter = value;
            Skip = (value.PageNumber - 1) * value.PageSize;
            Take = value.PageSize;

            IsPagingEnabled = true;
        }
    }
    
    /// <inheritdoc />
    public bool IsPagingEnabled { get; set; }

    
    /// <inheritdoc />
    public IEnumerable<OrderExpressionInfo<T>>? OrderExpressions
    {
        get;
        set;
    }

    /// <inheritdoc />
    public Expression<Func<T, object>>? GroupByExpression { get; set; }

    /// <inheritdoc />
    public int? Take { get; set; }

    /// <inheritdoc />
    public int? Skip { get; set; }

    /// <inheritdoc/>
    public bool IgnoreQueryFilters { get; set; }
    
    /// <summary>
    /// Inner <see cref="ISpecificationBuilder{T}"/>
    /// </summary>
    protected IBasicSpecificationBuilder<T> Query { get; }
    
    /// <summary>
    /// Inner <see cref="InMemorySpecificationEvaluator"/>
    /// </summary>
    protected IInMemorySpecificationEvaluator Evaluator { get; }
    
    /// <summary>
    /// Innner <see cref="ISpecificationEvaluator"/>
    /// </summary>
    protected ISpecificationValidator Validator { get; }

    /// <inheritdoc/>
    public virtual bool IsSatisfiedBy(T entity) 
        => Validator.IsValid(entity, this);

    /// <summary>
    /// Evaluates given <see cref="IEnumerable{T}"/> using self.
    /// </summary>
    /// <param name="entities">Entities to evaluate</param>
    /// <returns></returns>
    public virtual IEnumerable<T> Evaluate(IEnumerable<T> entities) 
        => Evaluator.Evaluate(entities, this);

    /// <inheritdoc />
    public IEnumerable<SearchExpressionInfo<T>>? SearchCriterias
    {
        get;
        set;
    }
    
    /// <inheritdoc />
    public IEnumerable<WhereExpressionInfo<T>>? WhereExpressions { get; set; }
    
    /// <summary>
    /// Specify a predicate that will be applied to the query
    /// </summary>
    /// <param name="criteria">Given criteria</param>
    /// <returns>Current <see cref="IBasicSpecificationBuilder{T}"/> instance</returns>
    protected virtual IBasicSpecificationBuilder<T> Where(Expression<Func<T, bool>> criteria) 
        => Query.Where(criteria);

    /// <summary>
    /// Specify a 'SQL LIKE' operations for search purposes
    /// </summary>
    /// <param name="selector">the property to apply the SQL LIKE against</param>
    /// <param name="searchTerm">the value to use for the SQL LIKE</param>
    /// <param name="searchGroup">the index used to group sets of Selectors and SearchTerms together</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected virtual IBasicSpecificationBuilder<T> Search(Expression<Func<T, string>> selector, string searchTerm, int searchGroup = 1) 
        => Query.Search(selector, searchTerm, searchGroup);

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderByExpression"/> in an ascending order
    /// </summary>
    /// <param name="orderByExpression">Member to use for ordering</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected virtual IOrderedBasicSpecificationBuilder<T> OrderBy(Expression<Func<T, object?>> orderByExpression) 
        => Query.OrderBy(orderByExpression);

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderByDescendingExpression"/> in a descending order
    /// </summary>
    /// <param name="orderByDescendingExpression">Member to use for ordering</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected virtual IOrderedBasicSpecificationBuilder<T> OrderByDescending(Expression<Func<T, object?>> orderByDescendingExpression) 
        => Query.OrderByDescending(orderByDescendingExpression);

    /// <summary>
    /// Specify the query result will be grouped by <paramref name="groupByExpression"/> in a descending order
    /// </summary>
    /// <param name="groupByExpression">Member to use for grouping</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected virtual IGroupedBasicSpecificationBuilder<T> GroupBy(Expression<Func<T, object>> groupByExpression) 
        => Query.GroupBy(groupByExpression);

    /// <summary>
    /// Specify a <see cref="PaginationFilter"/> to use
    /// </summary>
    /// <param name="paginationFilter"><see cref="PaginationFilter"/> to use</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected virtual IBasicSpecificationBuilder<T> WithPaginationFilter(PaginationFilter paginationFilter) 
        => Query.WithPaginationFilter(paginationFilter);

    /// <summary>
    /// Specify the number of elements to return.
    /// </summary>
    /// <param name="take">number of elements to take</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected virtual IBasicSpecificationBuilder<T> ApplyTake(int take) 
        => Query.Take(take);

    /// <summary>
    /// Specify the number of elements to skip before returning the remaining elements.
    /// </summary>
    /// <param name="skip">number of elements to skip</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected virtual IBasicSpecificationBuilder<T> ApplySkip(int skip) 
        => Query.Skip(skip);
}
