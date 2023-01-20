using EFCoreSecondLevelCacheInterceptor;

namespace DataExplorer.EfCore.Specifications.Builders;

[PublicAPI]
public static class CacheBuilderExtensions
{
    /// <summary>
    /// Specify an <see cref="ICacheSpecificationBuilder{T}"/> for this query
    /// </summary>
    /// <returns>Current <see cref="CacheExpirationMode"/> instance</returns>
    public static ICacheSpecificationBuilder<TEntity> WithExpirationMode<TEntity>(this ICacheSpecificationBuilder<TEntity> builder, CacheExpirationMode mode) where TEntity : class
    {
        builder.Specification.CacheExpirationMode = mode;

        return builder;
    }

    /// <summary>
    /// Specify a cache expiration timeout <see cref="TimeSpan"/> for this query
    /// </summary>
    /// <returns>Current <see cref="ICacheSpecificationBuilder{T}"/> instance</returns>
    public static ICacheSpecificationBuilder<TEntity> WithExpirationTimeout<TEntity>(this ICacheSpecificationBuilder<TEntity> builder, TimeSpan timeout) where TEntity : class
    {
        builder.Specification.CacheTimeout = timeout;

        return builder;
    }
}
