using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Evaluators;
using EFCoreSecondLevelCacheInterceptor;

namespace DataExplorer.EfCore.Specifications.Evaluators;

public class CachingEvaluator : IEvaluator, IEvaluatorBase
{
    private CachingEvaluator()
    {
    }

    public static CachingEvaluator Default { get; } = new();

    public bool IsCriteriaEvaluator { get; } = false;
    public int ApplicationOrder { get; } = 0;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (!specification.IsCacheEnabled.HasValue) 
            return query;
        
        if (!specification.IsCacheEnabled.Value)
            return query.NotCacheable();
        
        if (specification.CacheExpirationMode is null && specification.CacheTimeout is null)
            return query.Cacheable();
            
        if (specification.CacheExpirationMode is not null && specification.CacheTimeout is not null)
            return query.Cacheable((CacheExpirationMode)(int)specification.CacheExpirationMode.Value, specification.CacheTimeout.Value);

        return query.Cacheable();
    }
}
