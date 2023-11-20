namespace DataExplorer.EfCore.Specifications.Builders;

public class SpecificationBuilder<T, TResult> : SpecificationBuilder<T>, ISpecificationBuilder<T, TResult> where T : class
{
    public SpecificationBuilder(Specification<T, TResult> specification)
        : base(specification)
    {
        Specification = specification;
    }

    public override Specification<T, TResult> Specification { get; }
}

public class SpecificationBuilder<T> : BasicSpecificationBuilder<T>, ISpecificationBuilder<T> where T : class
{
    public SpecificationBuilder(Specification<T> specification) : base(specification)
    {
        Specification = specification;
    }
    
    public override Specification<T> Specification { get; }
}

#if NET7_0_OR_GREATER 
public class UpdateSpecificationBuilder<T> : BasicSpecificationBuilder<T>, IUpdateSpecificationBuilder<T> where T : class
{
    public UpdateSpecificationBuilder(UpdateSpecification<T> specification) : base(specification)
    {
        Specification = specification;
    }

    public override UpdateSpecification<T> Specification { get; }
}
#endif

public class BasicSpecificationBuilder<T> : IBasicSpecificationBuilder<T> where T : class
{
    public BasicSpecificationBuilder(BasicSpecification<T> specification)
    {
        Specification = specification;
    }

    public virtual BasicSpecification<T> Specification { get; }
}

