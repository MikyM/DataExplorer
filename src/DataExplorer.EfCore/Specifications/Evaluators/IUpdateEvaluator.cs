using System.Linq.Expressions;

namespace DataExplorer.EfCore.Specifications.Evaluators;

public interface IUpdateEvaluator : IEvaluatorData
{
    Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> Evaluate<T>(IUpdateSpecification<T> specification) where T : class;
}
