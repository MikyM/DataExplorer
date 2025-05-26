using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.EfCore.Abstractions;
using DataExplorer.EfCore.Abstractions.DataServices;
using DataExplorer.EfCore.Extensions;
using DataExplorer.Tests.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataExplorer.EfCore.Tests.Unit;

[CollectionDefinition("DependencyInjectionTests")]
public class DependencyInjectionTests
{
    [Collection("DependencyInjectionTests")]
    public class ContainerShould
    {
        [Theory]
        [InlineData(typeof(IUnitOfWork<ITestContext>))]
        [InlineData(typeof(IOptions<DataExplorerConfiguration>))]
        [InlineData(typeof(DataExplorerConfiguration))]
        [InlineData(typeof(DataExplorerEfCoreConfiguration))]
        [InlineData(typeof(IOptions<DataExplorerEfCoreConfiguration>))]
        [InlineData(typeof(IReadOnlyDataService<TestEntity, ITestContext>))]
        [InlineData(typeof(IReadOnlyDataService<TestEntity, long, ITestContext>))]
        [InlineData(typeof(ICrudDataService<TestEntity, ITestContext>))]
        [InlineData(typeof(ICrudDataService<TestEntity, long, ITestContext>))]
        [InlineData(typeof(ISpecificationEvaluator))]
        public void Resolve(Type typeToResolve)
        {
            // Arrange

            var services = new ServiceCollection();
            services.AddDbContext<ITestContext,TestContext>(x => x.UseInMemoryDatabase("test"));
            services.AddDataExplorer(x =>
            {
                x.AddEfCore(e =>
                {

                }, typeof(TestEntity).Assembly);
            });
            
            var provider = services.BuildServiceProvider();
            
            // Act && Assert
            
            var target = () => provider.GetRequiredService(typeToResolve);
            target.Should().NotThrow();
        }
    }
}