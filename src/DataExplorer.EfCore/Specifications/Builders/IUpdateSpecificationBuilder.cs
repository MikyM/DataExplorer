using DataExplorer.Abstractions.Specifications.Builders;
using DataExplorer.EfCore.Abstractions.Specifications;

#if NET7_0_OR_GREATER 
namespace DataExplorer.EfCore.Specifications.Builders;

public interface IUpdateSpecificationBuilder<T> : IBasicSpecificationBuilder<T> where T : class
{
    new IUpdateSpecification<T> Specification { get; }
}
#endif

