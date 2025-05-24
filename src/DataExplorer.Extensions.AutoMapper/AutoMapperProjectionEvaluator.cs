using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Evaluators;
using JetBrains.Annotations;

namespace DataExplorer.Extensions.AutoMapper;

/// <summary>
/// An implementation of <see cref="IProjectionEvaluator"/> based on AutoMapper.
/// </summary>
[PublicAPI]
[UsedImplicitly]
public class AutoMapperProjectionEvaluator : IProjectionEvaluator, ISpecialCaseEvaluator
{
    private readonly IMapper _mapper;
    
    internal AutoMapperProjectionEvaluator(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification) where T : class
    {
        if (_mapper.ConfigurationProvider is null)
            throw new InvalidOperationException();
        
        if (specification.MembersToExpand is not null)
            return query.ProjectTo(_mapper.ConfigurationProvider,
                    specification.MembersToExpand.ToArray());

        if (specification.StringMembersToExpand is not null)
        {
            return query.ProjectTo<TResult>(_mapper.ConfigurationProvider, null,
                specification.StringMembersToExpand.ToArray());
        }

        return query.ProjectTo<TResult>(_mapper.ConfigurationProvider);
    }
}
