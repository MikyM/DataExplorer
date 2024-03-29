﻿#if NET7_0_OR_GREATER

using System.Linq.Expressions;
using DataExplorer.Abstractions.Specifications.Builders;
using DataExplorer.EfCore.Abstractions.Specifications;
using DataExplorer.EfCore.Specifications.Builders;

namespace DataExplorer.EfCore.Specifications;

/// <inheritdoc cref="IUpdateSpecification{T}"/>
[PublicAPI]
public class UpdateSpecification<T> : BasicSpecification<T>, IUpdateSpecification<T> where T : class
{
    public UpdateSpecification()
    {
        Query = new UpdateSpecificationBuilder<T>(this);
    }
    
    public IEnumerable<Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>>? UpdateExpressions { get; set; }
    
    /// <summary>
    /// Inner <see cref="IUpdateSpecificationBuilder{T}"/>
    /// </summary>
    protected new Builders.IUpdateSpecificationBuilder<T> Query { get; }

    /// <summary>
    ///         Specify property and value to be set in ExecuteUpdate method with chaining multiple calls for updating
    ///         multiple columns.
    /// </summary>
    /// <param name="setPropertyCalls">The updates to execute.</param>
    public UpdateSpecification<T> Modify(Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls)
    {
        Query.Modify(setPropertyCalls);
        return this;
    }

    /// <summary>
    ///         Specify property and value to be set in ExecuteUpdate method with chaining multiple calls for updating
    ///         multiple columns.
    /// </summary>
    /// <param name="setPropertyCalls">The updates to execute.</param>
    /// <param name="condition">If false, the criteria won't be added.</param>
    public UpdateSpecification<T> Modify(Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls, bool condition)
    {
        Query.Modify(setPropertyCalls, condition);
        return this;
    }
}

#endif
