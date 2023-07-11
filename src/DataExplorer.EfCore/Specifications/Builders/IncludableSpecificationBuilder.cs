﻿namespace DataExplorer.EfCore.Specifications.Builders;

[PublicAPI]
public class IncludableSpecificationBuilder<T, TProperty> : IIncludableSpecificationBuilder<T, TProperty> where T : class
{
    public BasicSpecification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public IncludableSpecificationBuilder(BasicSpecification<T> specification, bool isChainDiscarded = false)
    {
        Specification = specification;
        IsChainDiscarded = isChainDiscarded;
    }
    
    BasicSpecification<T> IBasicSpecificationBuilder<T>.Specification => Specification;
}
