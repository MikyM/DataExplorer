using DataExplorer.EfCore.Specifications.Extensions;

namespace DataExplorer.EfCore.Specifications.Validators;

public class SearchValidator : IValidator, IBasicValidator, IValidatorMarker
{
    private SearchValidator() { }
    public static SearchValidator Instance { get; } = new();

    public bool IsValid<T>(T entity, ISpecification<T> specification) where T : class
        => IsValid(entity, (IBasicSpecification<T>)specification);
    
    public bool IsValid<T>(T entity, IBasicSpecification<T> specification) where T : class
    {
        if (specification.SearchCriterias is null) return true;

        foreach (var searchGroup in specification.SearchCriterias.GroupBy(x => x.SearchGroup))
        {
            if (searchGroup.Any(c => c.SelectorFunc(entity).Like(c.SearchTerm)) == false) return false;
        }

        return true;
    }
}
