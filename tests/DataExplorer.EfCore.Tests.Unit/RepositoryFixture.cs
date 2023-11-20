using AutoMapper;
using DataExplorer.EfCore.Abstractions.DataContexts;
using DataExplorer.EfCore.Abstractions.Repositories;
using DataExplorer.EfCore.Gridify;
using DataExplorer.EfCore.Repositories;
using DataExplorer.EfCore.Specifications.Evaluators;
using DataExplorer.Entities;
using Moq;

namespace DataExplorer.EfCore.Tests.Unit;

public class RepositoryFixture
{
    public Mock<ISpecificationEvaluator> GetEvaluatorMock() => new();
    public Mock<IMapper> GetMapperMock() => new();
    public Mock<IGridifyMapperProvider> GetGridifyMock() => new();
    
    public ReadOnlyRepository<TEntity> GetReadOnlyRepository<TEntity>(IEfDbContext efDbContext,
        ISpecificationEvaluator evaluator, IMapper mapper, IGridifyMapperProvider gridifyMapperProvider)
        where TEntity : Entity<long> =>
        new(efDbContext, evaluator, mapper, gridifyMapperProvider);
    
    public Repository<TEntity> GetRepository<TEntity>(IEfDbContext efDbContext, ISpecificationEvaluator evaluator,
        IMapper mapper, IGridifyMapperProvider gridifyMapperProvider) where TEntity : Entity<long> =>
        new(efDbContext, evaluator, mapper, gridifyMapperProvider);
    
    public ReadOnlyRepository<TEntity> GetReadOnlyRepository<TEntity>(Mock<IEfDbContext> efDbContext, Mock<ISpecificationEvaluator> evaluator, 
        Mock<IMapper> mapper, Mock<IGridifyMapperProvider> gridifyMapperProvider) where TEntity : Entity<long>
        => new(efDbContext.Object, evaluator.Object, mapper.Object, gridifyMapperProvider.Object);
    
    public Repository<TEntity> GetRepository<TEntity>(Mock<IEfDbContext> efDbContext, Mock<ISpecificationEvaluator> evaluator, 
        Mock<IMapper> mapper, Mock<IGridifyMapperProvider> gridifyMapperProvider) where TEntity : Entity<long>
        => new(efDbContext.Object, evaluator.Object, mapper.Object, gridifyMapperProvider.Object);
}
