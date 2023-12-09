using DataExplorer.Abstractions.Specifications;
using DataExplorer.Abstractions.Specifications.Builders;
using DataExplorer.EfCore.Abstractions.Specifications;

namespace DataExplorer.EfCore.Specifications.Builders;

public class SpecificationBuilder<T, TResult> : SpecificationBuilder<T>, ISpecificationBuilder<T, TResult> where T : class
{
    public SpecificationBuilder(ISpecification<T, TResult> specification)
        : base(specification)
    {
        Specification = specification;
    }

    public override ISpecification<T, TResult> Specification { get; }
}

public class SpecificationBuilder<T> : BasicSpecificationBuilder<T>, ISpecificationBuilder<T> where T : class
{
    public SpecificationBuilder(ISpecification<T> specification) : base(specification)
    {
        Specification = specification;
    }
    
    public override ISpecification<T> Specification { get; }
}

#if NET7_0_OR_GREATER 
public class UpdateSpecificationBuilder<T> : BasicSpecificationBuilder<T>, IUpdateSpecificationBuilder<T> where T : class
{
    public UpdateSpecificationBuilder(IUpdateSpecification<T> specification) : base(specification)
    {
        Specification = specification;
    }

    public override IUpdateSpecification<T> Specification { get; }
}
#endif

public class BasicSpecificationBuilder<T> : IBasicSpecificationBuilder<T> where T : class
{
    public BasicSpecificationBuilder(IBasicSpecification<T> specification)
    {
        Specification = specification;
    }

    public virtual IBasicSpecification<T> Specification { get; }
}

