﻿namespace DataExplorer.EfCore.Specifications.Builders;

public class OrderedSpecificationBuilder<T> : IOrderedSpecificationBuilder<T> where T : class
{
    public BasicSpecification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public OrderedSpecificationBuilder(BasicSpecification<T> specification, bool isChainDiscarded = false)
    {
        Specification = specification;
        IsChainDiscarded = isChainDiscarded;
    }
    
    BasicSpecification<T> IBasicSpecificationBuilder<T>.Specification => Specification;
}
