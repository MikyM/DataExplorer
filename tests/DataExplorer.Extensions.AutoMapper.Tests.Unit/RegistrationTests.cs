using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using DataExplorer.Abstractions.Mapper;
using DataExplorer.Abstractions.Specifications.Evaluators;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace DataExplorer.Extensions.AutoMapper.Tests.Unit;

[UsedImplicitly]
[Collection("RegistrationTests")]
public class RegistrationTests
{
    public class MicrosoftServiceProvider
    {
        [Fact]
        public void ShouldContainAutoMapperBridge()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAutoMapper(x => {});
            
            // Act
            services.AddDataExplorer(x =>
            {
                x.UseAutoMapper();
            });
            
            var provider = services.BuildServiceProvider();
            
            // Assert
            var bridge = provider.GetRequiredService<IMapper>();

            bridge.Should().BeOfType<AutoMapperBridge>();
        }
        
        [Fact]
        public void ShouldContainAutoMapperProjectionEvaluator()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAutoMapper(x => {});
            
            // Act
            services.AddDataExplorer(x =>
            {
                x.UseAutoMapper();
            });
            
            var provider = services.BuildServiceProvider();
            
            // Assert
            var bridge = provider.GetRequiredService<IProjectionEvaluator>();

            bridge.Should().BeOfType<AutoMapperProjectionEvaluator>();
        }
    }

    public class AutofacServiceProvider
    {
        [Fact]
        public void ShouldContainAutoMapperBridge()
        {
            // Arrange
            var services = new ContainerBuilder();
            services.RegisterAutoMapper();
            
            // Act
            services.AddDataExplorer(x =>
            {
                x.UseAutoMapper();
            });

            var provider = services.Build();
            
            // Assert
            var bridge = provider.Resolve<IMapper>();

            bridge.Should().BeOfType<AutoMapperBridge>();
        }
        
        [Fact]
        public void ShouldContainAutoMapperProjectionEvaluator()
        {
            // Arrange
            var services = new ContainerBuilder();
            services.RegisterAutoMapper();
            
            // Act
            services.AddDataExplorer(x =>
            {
                x.UseAutoMapper();
            });
            
            var provider = services.Build();
            
            // Assert
            var bridge = provider.Resolve<IProjectionEvaluator>();

            bridge.Should().BeOfType<AutoMapperProjectionEvaluator>();
        }
    }
}