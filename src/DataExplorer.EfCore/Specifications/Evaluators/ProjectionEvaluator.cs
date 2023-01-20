using AutoMapper.QueryableExtensions;

namespace DataExplorer.EfCore.Specifications.Evaluators;

public class ProjectionEvaluator : IProjectionEvaluator, ISpecialCaseEvaluator
{
    public static ProjectionEvaluator Instance { get; } = new();

    internal ProjectionEvaluator()
    {
        
    }

    public IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification) where T : class
    {
        if (specification.MapperConfigurationProvider is null)
            throw new InvalidOperationException();
        
        if (specification.MembersToExpand is not null)
            return query.ProjectTo(specification.MapperConfigurationProvider,
                    specification.MembersToExpand.ToArray());

        if (specification.StringMembersToExpand is not null)
        {
            return query.ProjectTo<TResult>(specification.MapperConfigurationProvider, null,
                specification.StringMembersToExpand.ToArray());
        }

        return query.ProjectTo<TResult>(specification.MapperConfigurationProvider);
    }
}
