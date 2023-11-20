namespace DataExplorer.EfCore.Specifications.Builders;

public interface ISpecificationBuilder<T, TResult> : ISpecificationBuilder<T> where T : class
{
    new Specification<T, TResult> Specification { get; }
}

public interface ISpecificationBuilder<T> : IBasicSpecificationBuilder<T> where T : class
{
    new Specification<T> Specification { get; }
}

#if NET7_0_OR_GREATER 
public interface IUpdateSpecificationBuilder<T> : IBasicSpecificationBuilder<T> where T : class
{
    new UpdateSpecification<T> Specification { get; }
}
#endif

public interface IBasicSpecificationBuilder<T> where T : class
{
    BasicSpecification<T> Specification { get; }
}
