﻿using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Validators;

namespace DataExplorer.EfCore.Specifications.Validators;

public class WhereValidator : IValidator, IBasicValidator, IValidatorMarker
{
    private WhereValidator() { }
    public static WhereValidator Instance { get; } = new();

    public bool IsValid<T>(T entity, ISpecification<T> specification) where T : class
        => IsValid(entity, (IBasicSpecification<T>)specification);
    
    public bool IsValid<T>(T entity, IBasicSpecification<T> specification) where T : class
    {
        if (specification.WhereExpressions is null) return true;

        foreach (var info in specification.WhereExpressions)
        {
            if (info.FilterFunc(entity) == false) return false;
        }

        return true;
    }
}
