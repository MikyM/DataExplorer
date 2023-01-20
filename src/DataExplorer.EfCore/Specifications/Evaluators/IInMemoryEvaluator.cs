namespace DataExplorer.EfCore.Specifications.Evaluators;

[PublicAPI]
public interface IInMemoryEvaluator : IInMemoryEvaluatorData
{
    IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class;
}

[PublicAPI]
public interface IBasicInMemoryEvaluator : IInMemoryEvaluatorData
{
    IEnumerable<T> Evaluate<T>(IEnumerable<T> query, IBasicSpecification<T> specification) where T : class;
}

[PublicAPI]
public interface IInMemoryEvaluatorData
{
    int ApplicationOrder { get; }
}

[PublicAPI]
public interface IInMemoryEvaluatorMarker
{
}
