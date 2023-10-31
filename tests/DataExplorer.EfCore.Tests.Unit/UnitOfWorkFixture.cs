using AutoMapper;
using DataExplorer.EfCore.Gridify;
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
    
    public Mock<IGridifyMapperProvider> GetIGridifyMapperProviderMock()
    {
        var gridifyMapperProviderMock = new Mock<IGridifyMapperProvider>();
        return gridifyMapperProviderMock;
    }
    
    public Mock<ISpecificationEvaluator> GetISpecificationEvaluatorMock()
    {
        var specificationEvaluatorMock = new Mock<ISpecificationEvaluator>();
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
