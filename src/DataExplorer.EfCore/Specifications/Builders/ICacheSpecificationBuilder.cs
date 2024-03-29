﻿using DataExplorer.Abstractions.Specifications.Builders;

namespace DataExplorer.EfCore.Specifications.Builders;

public interface ICacheSpecificationBuilder<T> : ISpecificationBuilder<T> where T : class
{
    bool IsChainDiscarded { get; set; }
}
