using DataExplorer.Abstractions.Mapper;
using JetBrains.Annotations;
using IAutoMapper = AutoMapper.IMapper;

namespace DataExplorer.Extensions.AutoMapper;

/// <summary>
/// An implementation of <see cref="IMapper"/> for AutoMapper.
/// </summary>
[PublicAPI]
[UsedImplicitly]
public class AutoMapperBridge : IMapper
{
    private readonly IAutoMapper _autoMapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="autoMapper">An AutoMapper instance.</param>
    internal AutoMapperBridge(IAutoMapper autoMapper)
    {
        _autoMapper = autoMapper;
    }

    /// <inheritdoc/>
    public TTarget Map<TTarget>(object source)
        => _autoMapper.Map<TTarget>(source);

    /// <inheritdoc/>
    public IQueryable<TTarget> ProjectTo<TTarget>(IQueryable source, IDictionary<string, object> parameters,
        params string[] membersToExpand)
        => _autoMapper.ProjectTo<TTarget>(source, parameters, membersToExpand);
}