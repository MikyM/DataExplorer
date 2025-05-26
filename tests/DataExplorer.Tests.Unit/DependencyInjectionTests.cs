using DataExplorer.Abstractions.Mapper;
using DataExplorer.IdGenerator;
using DataExplorer.Services;
using DataExplorer.Tests.Shared;
using DataExplorer.Utilities;
using IdGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DataExplorer.Tests.Unit;

[CollectionDefinition("DependencyInjectionTests")]
public class DependencyInjectionTests
{
    [Collection("DependencyInjectionTests")]
    public class ContainerShould
    {
        [Theory]
        [InlineData(typeof(IMapper))]
        [InlineData(typeof(ICachedInstanceFactory))]
        [InlineData(typeof(TimeProvider))]
        [InlineData(typeof(DataExplorerTimeProvider))]
        [InlineData(typeof(IOptions<DataExplorerConfiguration>))]
        [InlineData(typeof(DataExplorerConfiguration))]
        public void Resolve(Type typeToResolve)
        {
            // Arrange

            var services = new ServiceCollection();
            services.AddDbContext<ITestContext,TestContext>(x => x.UseInMemoryDatabase("test"));
            services.AddDataExplorer(x => { });
            
            var provider = services.BuildServiceProvider();
            
            // Act && Assert
            
            var target = () => provider.GetRequiredService(typeToResolve);
            target.Should().NotThrow();
        }

        [Fact]
        public void ResolveCorrectDefaultMapper()
        {
            // Arrange

            var services = new ServiceCollection();
            services.AddDbContext<ITestContext,TestContext>(x => x.UseInMemoryDatabase("test"));
            services.AddDataExplorer(x => { });
            
            var provider = services.BuildServiceProvider();
            
            // Act
            
            var target = provider.GetRequiredService<IMapper>();
            
            // Assert
            target.Should().BeOfType<DefaultMapper>();
        }
        
        [Theory]
        [InlineData(typeof(ISnowflakeGenerator))]
        [InlineData(typeof(IIdGenerator<long>))]
        public void ResolveSnowflakeGenerationServices(Type type)
        {
            // Arrange

            var services = new ServiceCollection();
            services.AddDbContext<ITestContext,TestContext>(x => x.UseInMemoryDatabase("test"));
            services.AddDataExplorer(x =>
            {
                x.AddSnowflakeIdGeneration();
            });
            
            var provider = services.BuildServiceProvider();
            
            // Act && Assert
            
            var func = () => provider.GetRequiredService(type);
            func.Should().NotThrow();
        }
        
        [Fact]
        public void ContainSnowflakeFactoryRegistrator()
        {
            // Arrange

            var services = new ServiceCollection();

            services.AddDbContext<ITestContext, TestContext>(x => x.UseInMemoryDatabase("test"));
            
            
            services.AddDataExplorer(x =>
            {
                x.AddSnowflakeIdGeneration();
            });
            
            var provider = services.BuildServiceProvider();
            
            // Act && Assert

            var hostedServices = provider.GetRequiredService<IEnumerable<IHostedService>>();
            hostedServices.Should().NotBeEmpty().And.ContainSingle(x => x.GetType() == typeof(SnowflakeIdFactoryRegistrator));
        }
    }
}