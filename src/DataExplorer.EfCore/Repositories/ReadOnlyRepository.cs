﻿using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataExplorer.EfCore.Gridify;
using DataExplorer.EfCore.Specifications;
using Gridify;
using Gridify.EntityFramework;
using ISpecificationEvaluator = DataExplorer.EfCore.Specifications.Evaluators.ISpecificationEvaluator;

namespace DataExplorer.EfCore.Repositories;

/// <summary>
/// Read-only repository.
/// </summary>
/// <inheritdoc cref="IReadOnlyRepository{TEntity}"/>
[PublicAPI]
public class ReadOnlyRepository<TEntity,TId> : IReadOnlyRepository<TEntity,TId> where TEntity : Entity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <inheritdoc />
    public Type EntityType => typeof(TEntity);

    /// <inheritdoc />
    IDataContextBase IRepositoryBase.Context => Context;

    /// <inheritdoc />
    public IEfDbContext Context { get; }
    
    /// <inheritdoc />
    public DbSet<TEntity> Set { get; }
    
    /// <inheritdoc />
    public IGridifyMapperProvider GridifyMapperProvider { get; }

    /// <summary>
    /// Specification evaluator.
    /// </summary>
    public ISpecificationEvaluator SpecificationEvaluator { get; }

    /// <summary>
    /// Mapper instance.
    /// </summary>
    protected readonly IMapper Mapper;

    /// <summary>
    /// Internal ctor.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="specificationEvaluator"></param>
    /// <param name="mapper">Mapper.</param>
    /// <param name="gridifyProvider">Gridify provider</param>
    /// <exception cref="ArgumentNullException"></exception>
    internal ReadOnlyRepository(IEfDbContext context, ISpecificationEvaluator specificationEvaluator, IMapper mapper,
        IGridifyMapperProvider gridifyProvider)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Set = context.Set<TEntity>();
        SpecificationEvaluator = specificationEvaluator;
        Mapper = mapper;
        GridifyMapperProvider = gridifyProvider;
    }

    /// <inheritdoc />
    public virtual ValueTask<TEntity?> GetAsync(params object[] keyValues)
        => Set.FindAsync(keyValues);
    
    /// <inheritdoc />
    public virtual ValueTask<TEntity?> GetAsync(object?[]? keyValues, CancellationToken cancellationToken)
        => Set.FindAsync(keyValues, cancellationToken);

    /// <inheritdoc />
    public virtual Task<TEntity?> GetSingleBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification)
            .FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc />
    public virtual Task<TResult?> GetSingleBySpecAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
    {
        specification.MapperConfigurationProvider ??= Mapper.ConfigurationProvider;
        
        return ApplySpecification(specification)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetSingleAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
        => GetSingleBySpecAsync(specification, cancellationToken);

    /// <inheritdoc />
    public virtual Task<TResult?> GetSingleAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        => GetSingleBySpecAsync(specification, cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<TEntity> AsAsyncEnumerable(ISpecification<TEntity> specification)
        => ApplySpecification(specification, true).AsAsyncEnumerable();

    /// <inheritdoc />
    public IAsyncEnumerable<TEntity> AsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate)
        => Set.Where(predicate).AsAsyncEnumerable();
    
    /// <inheritdoc />
    public IAsyncEnumerable<TEntity> AsAsyncEnumerable()
        => Set.AsAsyncEnumerable();

    /// <inheritdoc />
    public virtual Task<Paging<TEntity>> GetByGridifyQueryAsync(IGridifyQuery gridifyQuery,
        CancellationToken cancellationToken = default)
        => Set.GridifyAsync(gridifyQuery, cancellationToken, GridifyMapperProvider.GetMapperFor<TEntity>());

    /// <inheritdoc />
    public virtual Task<Paging<TEntity>> GetByGridifyQueryAsync(IGridifyQuery gridifyQuery,
        IGridifyMapper<TEntity> gridifyMapper, CancellationToken cancellationToken = default)
        => Set.GridifyAsync(gridifyQuery, cancellationToken, gridifyMapper);

    /// <inheritdoc />
    public virtual async Task<Paging<TResult>> GetByGridifyQueryAsync<TResult>(IGridifyQuery gridifyQuery,
        CancellationToken cancellationToken = default)
    {
        var queryable = await Set.GridifyQueryableAsync(gridifyQuery, GridifyMapperProvider.GetMapperFor<TEntity>(), cancellationToken);
        var sub = await queryable.Query.ProjectTo<TResult>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        return new Paging<TResult>(queryable.Count, sub.AsReadOnly());
    }

    /// <inheritdoc />
    public virtual async Task<Paging<TResult>> GetByGridifyQueryAsync<TResult>(IGridifyQuery gridifyQuery,
        IGridifyMapper<TEntity> gridifyMapper, CancellationToken cancellationToken = default)
    {
        var queryable = await Set.GridifyQueryableAsync(gridifyQuery, gridifyMapper, cancellationToken);
        var sub = await queryable.Query.ProjectTo<TResult>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        return new Paging<TResult>(queryable.Count, sub.AsReadOnly());
    }
    
    /// <inheritdoc />
    public virtual Task<Paging<TEntity>> GetAsync(IGridifyQuery gridifyQuery,
        CancellationToken cancellationToken = default)
        => GetByGridifyQueryAsync(gridifyQuery, cancellationToken);
    /// <inheritdoc />
    public virtual Task<Paging<TEntity>> GetAsync(IGridifyQuery gridifyQuery,
        IGridifyMapper<TEntity> gridifyMapper, CancellationToken cancellationToken = default)
        => GetByGridifyQueryAsync(gridifyQuery, gridifyMapper, cancellationToken);

    /// <inheritdoc />
    public virtual Task<Paging<TResult>> GetAsync<TResult>(IGridifyQuery gridifyQuery,
        CancellationToken cancellationToken = default)
        => GetByGridifyQueryAsync<TResult>(gridifyQuery, cancellationToken);

    /// <inheritdoc />
    public virtual Task<Paging<TResult>> GetAsync<TResult>(IGridifyQuery gridifyQuery,
        IGridifyMapper<TEntity> gridifyMapper, CancellationToken cancellationToken = default)
        => GetByGridifyQueryAsync<TResult>(gridifyQuery, gridifyMapper, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TResult>> GetBySpecAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
    {
        specification.MapperConfigurationProvider ??= Mapper.ConfigurationProvider;
        
        var result = await ApplySpecification(specification).ToListAsync(cancellationToken);
        return specification.PostProcessingAction is null
            ? result.AsReadOnly()
            : specification.PostProcessingAction(result).ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var result = await ApplySpecification(specification)
            .ToListAsync(cancellationToken);

        return specification.PostProcessingAction is null
            ? result.AsReadOnly()
            : specification.PostProcessingAction(result).ToList().AsReadOnly();
    }
    
    /// <inheritdoc />
    public virtual Task<IReadOnlyList<TResult>> GetAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        => GetBySpecAsync(specification, cancellationToken);

    /// <inheritdoc />
    public virtual Task<IReadOnlyList<TEntity>> GetAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
        => GetBySpecAsync(specification, cancellationToken);

    /// <inheritdoc />
    public virtual Task<long> LongCountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification)
            .LongCountAsync(cancellationToken);
    
    /// <inheritdoc />
    public virtual Task<long> LongCountAsync(CancellationToken cancellationToken = default)
        => Set.LongCountAsync(cancellationToken);
    
    /// <inheritdoc />
    public virtual Task<long> LongCountAsync(Expression<Func<TEntity,bool>> predicate, CancellationToken cancellationToken = default)
        => Set.LongCountAsync(predicate, cancellationToken);

    /// <inheritdoc />
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Set.AnyAsync(predicate, cancellationToken);

    /// <inheritdoc />
    public Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification).AnyAsync(cancellationToken);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Set.ToListAsync(cancellationToken);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TProjectTo>> GetAllAsync<TProjectTo>(CancellationToken cancellationToken = default) where TProjectTo : class
        => await ApplySpecification(new Specification<TEntity, TProjectTo>(Mapper.ConfigurationProvider))
            .ToListAsync(cancellationToken);

    /// <summary>
    ///     Filters the entities  of <typeparamref name="TEntity" />, to those that match the encapsulated query logic of the
    ///     <paramref name="specification" />.
    /// </summary>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <param name="evaluateCriteriaOnly">Whether to only evaluate criteria.</param>
    /// <returns>The filtered entities as an <see cref="IQueryable{T}" />.</returns>
    protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification,
        bool evaluateCriteriaOnly = false)
        => SpecificationEvaluator.GetQuery(Set.AsQueryable(), specification,
            evaluateCriteriaOnly);

    /// <summary>
    ///     Filters all entities of <typeparamref name="TEntity" />, that matches the encapsulated query logic of the
    ///     <paramref name="specification" />, from the database.
    ///     <para>
    ///         Projects each entity into a new form, being <typeparamref name="TResult" />.
    ///     </para>
    /// </summary>
    /// <typeparam name="TResult">The type of the value returned by the projection.</typeparam>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered projected entities as an <see cref="IQueryable{T}" />.</returns>
    protected virtual IQueryable<TResult> ApplySpecification<TResult>(ISpecification<TEntity, TResult> specification)
        => SpecificationEvaluator.GetQuery(Set.AsQueryable(), specification);
}


/// <summary>
/// Read-only repository.
/// </summary>
/// <inheritdoc cref="IReadOnlyRepository{TEntity}"/>
[PublicAPI]
public class ReadOnlyRepository<TEntity> : ReadOnlyRepository<TEntity, long>, IReadOnlyRepository<TEntity> where TEntity : Entity<long>
{
    internal ReadOnlyRepository(IEfDbContext context, ISpecificationEvaluator specificationEvaluator, IMapper mapper,
        IGridifyMapperProvider gridifyMapperProvider) : base(context, specificationEvaluator, mapper,
        gridifyMapperProvider)
    {
    }
}
