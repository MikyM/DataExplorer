using DataExplorer.Abstractions.Mapper;
using DataExplorer.EfCore.Specifications.Evaluators;
using DataExplorer.Utilities;
using Microsoft.Extensions.Options;
using Moq;

namespace DataExplorer.EfCore.Tests.Unit;

public class UnitOfWorkFixture
{
    public Mock<IMapper> GetIMapperMock()
    {
        var mapperMock = new Mock<IMapper>();
        return mapperMock;
    }
    
    public Mock<IEfSpecificationEvaluator> GetISpecificationEvaluatorMock()
    {
        var specificationEvaluatorMock = new Mock<IEfSpecificationEvaluator>();
        return specificationEvaluatorMock;
    }
    
    public Mock<ICachedInstanceFactory> GetICachedInstanceFactoryMock()
    {
        var cachedInstanceFactoryMock = new Mock<ICachedInstanceFactory>();
        return cachedInstanceFactoryMock;
    }
    
    public Mock<IEfDataExplorerTypeCache> GetIEfDataExplorerTypeCacheMock()
    {
        var efDataExplorerTypeCacheMock = new Mock<IEfDataExplorerTypeCache>();
        return efDataExplorerTypeCacheMock;
    }
    
    public Mock<IOptions<DataExplorerEfCoreConfiguration>> GetIOptionsMock()
    {
        var optionsMock = new Mock<IOptions<DataExplorerEfCoreConfiguration>>();
        return optionsMock;
    }
}
