﻿using System.Linq.Expressions;
using AutoMapper;
using DataExplorer.EfCore.Specifications.Builders;
using DataExplorer.EfCore.Specifications.Evaluators;
using DataExplorer.EfCore.Specifications.Expressions;
using DataExplorer.EfCore.Specifications.Validators;
using EFCoreSecondLevelCacheInterceptor;

namespace DataExplorer.EfCore.Specifications;

/// <inheritdoc cref="ISpecification{T,TResult}" />
[PublicAPI]
public class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult> where T : class
{
    protected internal Specification(IConfigurationProvider? mapperConfigurationProvider = null) : this(InMemorySpecificationEvaluator.Default, mapperConfigurationProvider)
    {
    }

    public Specification(PaginationFilter paginationFilter, IConfigurationProvider? mapperConfigurationProvider = null) : this(InMemorySpecificationEvaluator.Default, paginationFilter, mapperConfigurationProvider)
    {
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator, PaginationFilter paginationFilter, IConfigurationProvider? mapperConfigurationProvider = null) : base(
        inMemorySpecificationEvaluator, paginationFilter)
    {
        Query = new SpecificationBuilder<T, TResult>(this);
        MapperConfigurationProvider = mapperConfigurationProvider;
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator, IConfigurationProvider? mapperConfigurationProvider = null) : base(
        inMemorySpecificationEvaluator)
    {
        Query = new SpecificationBuilder<T, TResult>(this);
        MapperConfigurationProvider = mapperConfigurationProvider;
    }

    /// <summary>
    /// Inner <see cref="ISpecificationBuilder{T,TResult}"/>
    /// </summary>
    protected new ISpecificationBuilder<T, TResult> Query { get; }

    /// <inheritdoc />
    public new virtual IEnumerable<TResult> Evaluate(IEnumerable<T> entities)
    {
        return Evaluator.Evaluate(entities, this);
    }

    /// <inheritdoc/>
    public Expression<Func<T, TResult>>? Selector { get; internal set; }
    
    /// <inheritdoc/>
    public Expression<Func<T, IEnumerable<TResult>>>? SelectorMany { get; internal set; }

    /// <inheritdoc />
    public IConfigurationProvider? MapperConfigurationProvider { get; set; }

    /// <inheritdoc />
    public IEnumerable<Expression<Func<TResult, object>>>? MembersToExpand { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<string>? StringMembersToExpand { get; internal set; }

    /// <inheritdoc />
    public new Func<IEnumerable<TResult>, IEnumerable<TResult>>? PostProcessingAction { get; internal set; }

    /// <summary>
    /// Specify a transform function to apply to the result of the query.
    /// and returns another <typeparamref name="TResult"/> type
    /// </summary>
    protected Specification<T> WithPostProcessingAction(Func<IEnumerable<TResult>, IEnumerable<TResult>> postProcessingAction)
    {
        Query.WithPostProcessingAction(postProcessingAction);
        return this;
    }

    /// <summary>
    /// Adds automapper configuration.
    /// </summary>
    /// <param name="mapperConfiguration"><see cref="MapperConfiguration"/> instance</param>
    /// <returns>Current specification instance</returns>
    protected Specification<T> WithMapperConfiguration(MapperConfiguration mapperConfiguration)
    {
        Query.WithMapperConfiguration(mapperConfiguration);
        return this;
    }
    
    /// <summary>
    /// Specify a transform function to apply to the <typeparamref name="T"/> element 
    /// </summary>
    /// <param name="selector">Selector</param>
    /// <returns>Current specification instance</returns>
    protected Specification<T> Select(Expression<Func<T, TResult>> selector)
    {
        Query.Select(selector);
        return this;
    }
    
    /// <summary>
    /// Specify a transform function to apply to the <typeparamref name="T"/> element 
    /// to produce a flattened sequence of <typeparamref name="TResult"/> elements.
    /// </summary>
    protected Specification<T> SelectMany(Expression<Func<T, IEnumerable<TResult>>> selector)
    {
        Query.SelectMany(selector);
        return this;
    }

    /// <summary>
    /// Expands given member.
    /// </summary>
    /// <param name="expression">Member to expand</param>
    /// <returns>Current specification instance</returns>
    protected Specification<T> Expand(Expression<Func<TResult, object>> expression)
    {
        Query.Expand(expression);
        return this;
    }

    /// <summary>
    /// Expands given member.
    /// </summary>
    /// <param name="member">Member to expand</param>
    /// <returns>Current specification instance</returns>
    protected Specification<T> Expand(string member)
    {
        Query.Expand(member);
        return this;
    }
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
    public TimeSpan? CacheTimeout { get; internal set; }

    /// <inheritdoc />
    public CacheExpirationMode? CacheExpirationMode { get; internal set; }

    /// <inheritdoc />
    public Func<IEnumerable<T>, IEnumerable<T>>? PostProcessingAction { get; internal set; }

    /// <inheritdoc />
    public bool? IsCacheEnabled { get; internal set; }
    
    /// <inheritdoc />
    public bool IsAsNoTracking { get; internal set; } = true;

    /// <inheritdoc />
    public bool IsAsSplitQuery { get; internal set; }

    /// <inheritdoc />
    public bool IsAsNoTrackingWithIdentityResolution { get; internal set; }
    
    /// <summary>
    /// Inner <see cref="ISpecificationBuilder{T,TResult}"/>
    /// </summary>
    protected new ISpecificationBuilder<T> Query { get; }

    /// <summary>
    /// Sets whether to ignore query filters or not, default is
    /// </summary>
    /// <param name="ignore">Whether to ignore query filters</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T,TResult}"/> instance</returns>
    protected Specification<T> WithIgnoreQueryFilters(bool ignore = true)
    {
        Query.IgnoreQueryFilters(ignore);
        return this;
    }

    /// <summary>
    /// Specify a transform function to apply to the result of the query 
    /// and returns the same <typeparamref name="T"/> type
    /// </summary>
    /// <param name="postProcessingAction">Action to use for post processing</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected Specification<T> WithPostProcessingAction(Func<IEnumerable<T>, IEnumerable<T>> postProcessingAction)
    {
        Query.WithPostProcessingAction(postProcessingAction);
        return this;
    }

    /// <summary>
    /// Specify whether results should be cached using <see cref="SecondLevelCacheInterceptor"/>.
    /// </summary>
    /// <param name="withCaching">Whether to cache results</param>
    /// <returns><see cref="ICacheSpecificationBuilder{T}"/> instance</returns>
    protected Specification<T> WithCaching(bool withCaching = true)
    {
        Query.WithCaching(withCaching); 
        return this;
    }

    /// <summary>
    /// Specify whether to use tracking for this query (default setting is AsNoTracking)
    /// </summary>
    /// <returns><see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected Specification<T> AsTracking()
    {
        Query.AsTracking();
        return this;
    }

    /// <summary>
    /// Specify whether to use tracking use split query
    /// </summary>
    /// <returns><see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected Specification<T> AsSplitQuery()
    {
        Query.AsSplitQuery();
        return this;
    }

    /// <summary>
    /// Specify whether to use AsNoTrackingWithIdentityResolution
    /// </summary>
    /// <returns><see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected Specification<T> AsNoTrackingWithIdentityResolution()
    {
        Query.AsNoTrackingWithIdentityResolution();
        return this;
    }
}

/// <inheritdoc cref="ISpecification{T}" />
[PublicAPI]
public class BasicSpecification<T> : IBasicSpecification<T> where T : class
{
    protected BasicSpecification()
        : this(InMemorySpecificationEvaluator.Default, SpecificationValidator.Default)
    {
    }

    protected BasicSpecification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator)
        : this(inMemorySpecificationEvaluator, SpecificationValidator.Default)
    {
    }

    protected BasicSpecification(ISpecificationValidator specificationValidator)
        : this(InMemorySpecificationEvaluator.Default, specificationValidator)
    {
    }

    protected BasicSpecification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator, ISpecificationValidator specificationValidator)
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
        internal set
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
    public bool IsPagingEnabled { get; internal set; }

    
    /// <inheritdoc />
    public IEnumerable<OrderExpressionInfo<T>>? OrderExpressions
    {
        get;
        internal set;
    }

    /// <inheritdoc />
    public IEnumerable<IncludeExpressionInfo>? IncludeExpressions { get; internal set; }

    /// <inheritdoc />
    public Expression<Func<T, object>>? GroupByExpression { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<string>? IncludeStrings { get; internal set; }

    /// <inheritdoc />
    public int? Take { get; internal set; }

    /// <inheritdoc />
    public int? Skip { get; internal set; }

    /// <inheritdoc/>
    public bool IgnoreQueryFilters { get; internal set; }
    
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
    {
        return Validator.IsValid(entity, this);
    }

    /// <summary>
    /// Evaluates given <see cref="IEnumerable{T}"/> using self.
    /// </summary>
    /// <param name="entities">Entities to evaluate</param>
    /// <returns></returns>
    public virtual IEnumerable<T> Evaluate(IEnumerable<T> entities)
    {
        return Evaluator.Evaluate(entities, this);
    }
    
    /// <inheritdoc />
    public IEnumerable<SearchExpressionInfo<T>>? SearchCriterias
    {
        get;
        internal set;
    }
    
    /// <inheritdoc />
    public IEnumerable<WhereExpressionInfo<T>>? WhereExpressions { get; internal set; }
    
    /// <summary>
    /// Specify a predicate that will be applied to the query
    /// </summary>
    /// <param name="criteria">Given criteria</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected BasicSpecification<T> Where(Expression<Func<T, bool>> criteria)
    {
        Query.Where(criteria);
        return this;
    }

    /// <summary>
    /// Specify a 'SQL LIKE' operations for search purposes
    /// </summary>
    /// <param name="selector">the property to apply the SQL LIKE against</param>
    /// <param name="searchTerm">the value to use for the SQL LIKE</param>
    /// <param name="searchGroup">the index used to group sets of Selectors and SearchTerms together</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected BasicSpecification<T> Search(Expression<Func<T, string>> selector, string searchTerm, int searchGroup = 1)
    {
        Query.Search(selector, searchTerm, searchGroup);
        return this;
    }
    
        /// <summary>
    /// Specify an include expression.
    /// This information is utilized to build Include function in the query, which ORM tools like Entity Framework use
    /// to include related entities (via navigation properties) in the query result.
    /// </summary>
    /// <param name="includeExpression">Member to include</param>
    /// <returns><see cref="IIncludableSpecificationBuilder{T,TProperty}"/> instance</returns>
    protected BasicSpecification<T> Include<TProperty>(
        Expression<Func<T, TProperty?>> includeExpression)
    {
        Query.Include(includeExpression);
        return this;
    }

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderByExpression"/> in an ascending order
    /// </summary>
    /// <param name="orderByExpression">Member to use for ordering</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected BasicSpecification<T> OrderBy(Expression<Func<T, object?>> orderByExpression)
    {
        Query.OrderBy(orderByExpression);
        return this;
    }

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderByDescendingExpression"/> in a descending order
    /// </summary>
    /// <param name="orderByDescendingExpression">Member to use for ordering</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected BasicSpecification<T> OrderByDescending(Expression<Func<T, object?>> orderByDescendingExpression)
    {
        Query.OrderByDescending(orderByDescendingExpression);
        return this;
    }

    /// <summary>
    /// Specify the query result will be grouped by <paramref name="groupByExpression"/> in a descending order
    /// </summary>
    /// <param name="groupByExpression">Member to use for grouping</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected BasicSpecification<T> GroupBy(Expression<Func<T, object>> groupByExpression)
    {
        Query.GroupBy(groupByExpression);
        return this;
    }

    /// <summary>
    /// Specify a <see cref="PaginationFilter"/> to use
    /// </summary>
    /// <param name="paginationFilter"><see cref="PaginationFilter"/> to use</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected BasicSpecification<T> WithPaginationFilter(PaginationFilter paginationFilter)
    {
        Query.WithPaginationFilter(paginationFilter);
        return this;
    }

    /// <summary>
    /// Specify the number of elements to return.
    /// </summary>
    /// <param name="take">number of elements to take</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected BasicSpecification<T> ApplyTake(int take)
    {
        Query.Take(take);
        return this;
    }

    /// <summary>
    /// Specify the number of elements to skip before returning the remaining elements.
    /// </summary>
    /// <param name="skip">number of elements to skip</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected BasicSpecification<T> ApplySkip(int skip)
    {
        Query.Skip(skip);
        return this;
    }
}
