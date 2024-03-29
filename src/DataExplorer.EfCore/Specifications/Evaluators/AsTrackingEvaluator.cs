﻿using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Evaluators;

namespace DataExplorer.EfCore.Specifications.Evaluators;

public class AsTrackingEvaluator : IEvaluator, IEvaluatorBase
{
    private AsTrackingEvaluator() { }
    
    public static AsTrackingEvaluator Default { get; } = new();

    public bool IsCriteriaEvaluator { get; } = true;
    public int ApplicationOrder { get; } = 0;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.IsAsTracking)
        {
            query = query.AsTracking();
        }

        return query;
    }
}
