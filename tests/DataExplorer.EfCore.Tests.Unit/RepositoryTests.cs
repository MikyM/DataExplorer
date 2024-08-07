﻿using DataExplorer.Tests.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DataExplorer.EfCore.Tests.Unit;

public class RepositoryTests : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
{
    public class Context : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public Context(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Fact]
        public void ShouldReturnCorrectInstance()
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var repo = _repoFixture.GetRepository<TestEntity>(ctx, _repoFixture.GetEvaluatorMock(),
                _repoFixture.GetMapperMock());
        
            // Act & Assert
            repo.Context.Should().NotBeNull().And.Be(ctx.Object);
        }
    }
    
    public class SpecificationEvaluator : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public SpecificationEvaluator(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Fact]
        public void ShouldReturnCorrectInstance()
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
        
            var evaluator = _repoFixture.GetEvaluatorMock();
        
            var repo = _repoFixture.GetRepository<TestEntity>(ctx, evaluator,
                _repoFixture.GetMapperMock());
        
            // Act & Assert
            repo.SpecificationEvaluator.Should().NotBeNull().And.Be(evaluator.Object);
        }
    }
    
    public class Set : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public Set(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Fact]
        public void ShouldReturnCorrectInstance()
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var set = new Mock<DbSet<TestEntity>>();
            ctx.Setup(x => x.Set<TestEntity>()).Returns(set.Object);
        
            var evaluator = _repoFixture.GetEvaluatorMock();
        
            var repo = _repoFixture.GetRepository<TestEntity>(ctx, evaluator,
                _repoFixture.GetMapperMock());
        
            // Act & Assert
            repo.Set.Should().NotBeNull().And.BeSameAs(set.Object);
        }
    }
}
