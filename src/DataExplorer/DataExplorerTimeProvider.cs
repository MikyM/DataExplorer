namespace DataExplorer;

/// <summary>
/// The Data Explorer time provider schim.
/// </summary>
[PublicAPI]
public abstract class DataExplorerTimeProvider
{
    /// <summary>
    /// A singleton instance of <see cref="DataExplorerTimeProvider"/>.
    /// </summary>
    public static DataExplorerTimeProvider Instance { get; } = new StaticDataExplorerTimeProvider();
    
    /// <summary>
    /// Gets the local now.
    /// </summary>
    /// <returns>The local now.</returns>
    public abstract DateTimeOffset GetLocalNow() ;
    /// <summary>
    /// Gets the UTC now.
    /// </summary>
    /// <returns>The UTC now.</returns>
    public abstract DateTimeOffset GetUtcNow();
    
    /// <summary>
    /// The Data Explorer time provider schim based on <see cref="DateTimeOffset"/>.
    /// </summary>
    public class StaticDataExplorerTimeProvider : DataExplorerTimeProvider
    {
        /// <inheritdoc/>
        public override DateTimeOffset GetLocalNow() => DateTimeOffset.Now;

        /// <inheritdoc/>
        public override DateTimeOffset GetUtcNow()  => DateTimeOffset.UtcNow;
    }
    
#if NET8_0_OR_GREATER
    /// <summary>
    /// The Data Explorer time provider schim based on <see cref="TimeProvider"/>.
    /// </summary>
    public class DependencyDataExplorerTimeProvider : DataExplorerTimeProvider
    {
        private readonly TimeProvider _timeProvider;
        // ReSharper disable once ConvertToPrimaryConstructor
        public DependencyDataExplorerTimeProvider(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        /// <inheritdoc/>
        public override DateTimeOffset GetLocalNow() => _timeProvider.GetLocalNow();

        /// <inheritdoc/>
        public override DateTimeOffset GetUtcNow() => _timeProvider.GetUtcNow();
    }
#endif
}
