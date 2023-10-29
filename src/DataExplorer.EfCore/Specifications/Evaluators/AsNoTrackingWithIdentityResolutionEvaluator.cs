﻿namespace DataExplorer.EfCore.Specifications.Evaluators;

public class AsNoTrackingWithIdentityResolutionEvaluator : IEvaluator, IEvaluatorMarker
{
    private AsNoTrackingWithIdentityResolutionEvaluator()
    {
    }

    public static AsNoTrackingWithIdentityResolutionEvaluator Default { get; } = new();

    public bool IsCriteriaEvaluator { get; } = true;
    public int ApplicationOrder { get; } = 0;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.IsAsNoTrackingWithIdentityResolution) query = query.AsNoTrackingWithIdentityResolution();

        return query;
    }
}
