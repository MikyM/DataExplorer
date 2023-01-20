namespace DataExplorer.EfCore.Specifications.Validators;

public interface ISpecificationValidator
{
    bool IsValid<T>(T entity, IBasicSpecification<T> specification) where T : class;
}
