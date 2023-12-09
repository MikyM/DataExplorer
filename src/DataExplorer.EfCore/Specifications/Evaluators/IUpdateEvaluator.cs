#if NET7_0_OR_GREATER

using System.Linq.Expressions;
using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.EfCore.Abstractions.Specifications;

namespace DataExplorer.EfCore.Specifications.Evaluators;

public interface IUpdateEvaluator : IEvaluatorData
{
    Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> Evaluate<T>(IUpdateSpecification<T> specification) where T : class;
}

#endif
