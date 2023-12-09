namespace DataExplorer.Abstractions.Specifications.Evaluators;


[PublicAPI]
public interface IEvaluatorData
{
    bool IsCriteriaEvaluator { get; }
    
    int ApplicationOrder { get; }
}
