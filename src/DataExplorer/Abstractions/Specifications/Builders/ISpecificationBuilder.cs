namespace DataExplorer.Abstractions.Specifications.Builders;

public interface ISpecificationBuilder<T, TResult> : ISpecificationBuilder<T> where T : class
{
    new ISpecification<T, TResult> Specification { get; }
}

public interface ISpecificationBuilder<T> : IBasicSpecificationBuilder<T> where T : class
{
    new ISpecification<T> Specification { get; }
}

public interface IBasicSpecificationBuilder<T> where T : class
{
    IBasicSpecification<T> Specification { get; }
}
