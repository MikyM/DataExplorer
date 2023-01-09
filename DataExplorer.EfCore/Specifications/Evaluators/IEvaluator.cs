namespace DataExplorer.EfCore.Specifications.Evaluators;

[PublicAPI]
public interface IPreUpdateEvaluator : IBasicEvaluator
{
    IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class;
}


[PublicAPI]
public interface IEvaluator : IEvaluatorData
{
    IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class;
}

[PublicAPI]
public interface IBasicEvaluator : IEvaluatorData
{
    IQueryable<T> GetQuery<T>(IQueryable<T> query, IBasicSpecification<T> specification) where T : class;
}

[PublicAPI]
public interface IEvaluatorData
{
    bool IsCriteriaEvaluator { get; }
    
    int ApplicationOrder { get; }
}
