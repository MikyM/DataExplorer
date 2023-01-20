namespace DataExplorer.EfCore.Specifications.Validators;

public class SpecificationValidator : ISpecificationValidator
{
    // Will use singleton for default configuration. Yet, it can be instantiated if necessary, with default or provided validators.
    public static SpecificationValidator Default { get; } = new();

    private readonly List<IValidator> _validators = new();
    private readonly List<IBasicValidator> _basicValidators = new();


    internal SpecificationValidator()
    {
        _validators.AddRange(new IValidator[]
        {
            WhereValidator.Instance,
            SearchValidator.Instance
        });
        
        _basicValidators.AddRange(new IBasicValidator[]
        {
            WhereValidator.Instance,
            SearchValidator.Instance
        });
    }
    internal SpecificationValidator(IEnumerable<IValidator> validators, IEnumerable<IBasicValidator> basicValidators)
    {
        _validators.AddRange(validators);
        _basicValidators.AddRange(basicValidators);
    }

    public virtual bool IsValid<T>(T entity, ISpecification<T> specification) where T : class
    {
        foreach (var partialValidator in _validators)
        {
            if (partialValidator.IsValid(entity, specification) == false) return false;
        }

        return true;
    }

    public bool IsValid<T>(T entity, IBasicSpecification<T> specification) where T : class
    {
        foreach (var partialValidator in _basicValidators)
        {
            if (partialValidator.IsValid(entity, specification) == false) return false;
        }

        return true;
    }
}
