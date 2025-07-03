using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataExplorer.Abstractions.Mapper;
using DataExplorer.Abstractions.Specifications.Evaluators;
using DataExplorer.EfCore;
using DataExplorer.EfCore.Abstractions;
using DataExplorer.EfCore.Abstractions.DataServices;
using DataExplorer.EfCore.Extensions;
using DataExplorer.IdGenerator;
using DataExplorer.Services;
using DataExplorer.Tests.Shared;
using DataExplorer.Utilities;
using FluentAssertions;
using IdGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DataExplorer.Extensions.Autofac.Tests.Unit;

[CollectionDefinition("DependencyInjectionTests")]
public class DependencyInjectionTests
{
    [Collection("DependencyInjectionTests")]
    public class ContainerShould
    {
        private static DbContextOptions<TContext> DbContextOptionsFactory<TContext>(
            Action<DbContextOptionsBuilder>? optionsAction)
            where TContext : DbContext
        {
            var options = new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>());
            if (optionsAction != null)
            {
                var builder = new DbContextOptionsBuilder<TContext>(options);
                optionsAction(builder);
                options = builder.Options;
            }

            return options;
        }   
        
        [Theory]
        [InlineData(typeof(IOptions<DataExplorerConfiguration>))]
        [InlineData(typeof(DataExplorerConfiguration))]
        [InlineData(typeof(DataExplorerEfCoreConfiguration))]
        [InlineData(typeof(IOptions<DataExplorerEfCoreConfiguration>))]
        [InlineData(typeof(IUnitOfWork<ITestContext>))]
        [InlineData(typeof(IReadOnlyDataService<TestEntity, ITestContext>))]
        [InlineData(typeof(IReadOnlyDataService<TestEntity, long, ITestContext>))]
        [InlineData(typeof(ICrudDataService<TestEntity, ITestContext>))]
        [InlineData(typeof(ICrudDataService<TestEntity, long, ITestContext>))]
        [InlineData(typeof(ISpecificationEvaluator))]
        [InlineData(typeof(IMapper))]
        [InlineData(typeof(ICachedInstanceFactory))]
        [InlineData(typeof(TimeProvider))]
        [InlineData(typeof(DataExplorerTimeProvider))]
        [InlineData(typeof(ISnowflakeGenerator))]
        [InlineData(typeof(IIdGenerator<long>))]
        public void Resolve(Type typeToResolve)
        {
            // Arrange

            var services = new ContainerBuilder();

            var microsoftServices = new ServiceCollection();

            microsoftServices.AddDbContext<ITestContext, TestContext>(x => x.UseInMemoryDatabase("test"));
            
            services.Populate(microsoftServices);
            
            services.AddDataExplorer(x =>
            {
                x.AddSnowflakeIdGeneration();
                x.AddEfCore(e =>
                {

                }, typeof(TestEntity).Assembly);
            });
            
            var provider = services.Build();
            
            // Act && Assert
            
            var target = () => provider.Resolve(typeToResolve);
            target.Should().NotThrow();
        }
        
        [Fact]
        public void ResolveTestContextRegisteredWithAddDbContext()
        {
            // Arrange

            var services = new ContainerBuilder();

            var microsoftServices = new ServiceCollection();

            microsoftServices.AddDbContext<ITestContext, TestContext>(x => x.UseInMemoryDatabase("test"));
            
            services.Populate(microsoftServices);
            
            services.AddDataExplorer(x =>
            {
                x.AddSnowflakeIdGeneration();
                x.AddEfCore(e =>
                {

                }, typeof(TestEntity).Assembly);
            });
            
            var provider = services.Build();
            
            // Act && Assert
            
            var target = () => provider.Resolve(typeof(ITestContext));
            target.Should().NotThrow();
        }
        
        [Fact]
        public void ResolveTestContextRegisteredWithAddDbContextPool()
        {
            // Arrange

            var services = new ContainerBuilder();

            var microsoftServices = new ServiceCollection();

            microsoftServices.AddDbContextPool<ITestContextPooled,TestContextPooled>(x => x.UseInMemoryDatabase("test"));
            
            services.Populate(microsoftServices);
            
            services.AddDataExplorer(x =>
            {
                x.AddSnowflakeIdGeneration();
                x.AddEfCore(e =>
                {

                }, typeof(TestEntity).Assembly);
            });
            
            var provider = services.Build();
            
            // Act && Assert
            
            var target = () => provider.Resolve(typeof(ITestContextPooled));
            target.Should().NotThrow();
        }

        [Fact]
        public void ContainSnowflakeFactoryRegistrator()
        {
            // Arrange

            var services = new ContainerBuilder();

            var microsoftServices = new ServiceCollection();

            microsoftServices.AddDbContext<ITestContext, TestContext>(x => x.UseInMemoryDatabase("test"));
            
            services.Populate(microsoftServices);
            
            services.AddDataExplorer(x =>
            {
                x.AddSnowflakeIdGeneration();
                x.AddEfCore(e =>
                {

                }, typeof(TestEntity).Assembly);
            });
            
            var provider = services.Build();
            
            // Act && Assert

            var hostedServices = provider.Resolve<IEnumerable<IHostedService>>();
            hostedServices.Should().NotBeEmpty().And.ContainSingle(x => x.GetType() == typeof(SnowflakeIdFactoryRegistrator));
        }
    }
}