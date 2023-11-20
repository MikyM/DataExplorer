using DataExplorer.EfCore.Specifications;
using DataExplorer.Tests.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace DataExplorer.EfCore.Tests.Unit;

public class ReadOnlyRepositoryTests : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
{
    public class GridifyMapperProvider : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GridifyMapperProvider(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Fact]
        public void ShouldReturnCorrectInstance()
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();

            var gridify = _repoFixture.GetGridifyMock();
        
            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx, _repoFixture.GetEvaluatorMock(),
                _repoFixture.GetMapperMock(), gridify);
        
            // Act & Assert
            repo.GridifyMapperProvider.Should().NotBeNull().And.Be(gridify.Object);
        }
    }

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

            var gridify = _repoFixture.GetGridifyMock();
        
            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx, _repoFixture.GetEvaluatorMock(),
                _repoFixture.GetMapperMock(), gridify);
        
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

            var gridify = _repoFixture.GetGridifyMock();
        
            var evaluator = _repoFixture.GetEvaluatorMock();
        
            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx, evaluator,
                _repoFixture.GetMapperMock(), gridify);
        
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

            var gridify = _repoFixture.GetGridifyMock();
        
            var evaluator = _repoFixture.GetEvaluatorMock();
        
            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx, evaluator,
                _repoFixture.GetMapperMock(), gridify);
        
            // Act & Assert
            repo.Set.Should().NotBeNull().And.BeSameAs(set.Object);
        }
    }

    public class GetAsyncParams : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GetAsyncParams(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallFindAsyncAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
            if (id is not null)
                test.SetId(id.Value);

            var returnVal = id.HasValue ? test : null;

            setMock.Setup(x => x.FindAsync(It.Is<object[]>(o => o.Length == 1))).ReturnsAsync(returnVal);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx, _repoFixture.GetEvaluatorMock(),
                _repoFixture.GetMapperMock(), _repoFixture.GetGridifyMock());

            // Act
            var result = await repo.GetAsync(id ?? 15);

            // Assert
            if (id is not null)
                result.Should().NotBeNull().And.Be(test);
            else
                result.Should().BeNull();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.FindAsync(It.Is<object[]>(o => o.Length == 1)), Times.Once);
        }
    }
    
    public class GetAsync : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GetAsync(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallFindAsyncAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
            if (id is not null)
                test.SetId(id.Value);

            var returnVal = id.HasValue ? test : null;

            setMock.Setup(x =>
                    x.FindAsync(It.Is<object?[]?>(o => o != null && o.Length == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnVal);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx, _repoFixture.GetEvaluatorMock(),
                _repoFixture.GetMapperMock(), _repoFixture.GetGridifyMock());

            // Act
            var result = await repo.GetAsync(new object?[] { id ?? 15 }, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().NotBeNull().And.Be(test);
            else
                result.Should().BeNull();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(
                x => x.FindAsync(It.Is<object?[]?>(o => o != null && o.Length == 1), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    public class GetSingleBySpecAsync : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GetSingleBySpecAsync(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
        
            var queryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
            var retQueryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
        
            setMock.Setup(x => x.AsQueryable())
                .Returns(queryable.Object);

            var spec = new TestSpecification(test.Id);

            var evaluator = _repoFixture.GetEvaluatorMock();
            evaluator.Setup(x => x.GetQuery(queryable.Object, spec, false)).Returns(retQueryable.Object);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.GetSingleBySpecAsync(spec, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().NotBeNull().And.Be(test);
            else
                result.Should().BeNull();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.AsQueryable(), Times.Once);
            evaluator.Verify(x =>  x.GetQuery(queryable.Object, spec, false), Times.Once);
        }
    }

    public class GetSingleBySpecAsyncGeneric : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GetSingleBySpecAsyncGeneric(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
            var test1 = new TestEntityOffset();
        
            var queryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
            var retQueryable = (id is null ? Array.Empty<TestEntityOffset>() : new [] { test1 }).AsQueryable().BuildMockDbSet();
        
            setMock.Setup(x => x.AsQueryable())
                .Returns(queryable.Object);

            var spec = new TestSpecificationTransform(test.Id);

            var evaluator = _repoFixture.GetEvaluatorMock();
            evaluator.Setup(x => x.GetQuery(queryable.Object, spec)).Returns(retQueryable.Object);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.GetSingleBySpecAsync(spec, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().NotBeNull().And.Be(test1);
            else
                result.Should().BeNull();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.AsQueryable(), Times.Once);
            evaluator.Verify(x =>  x.GetQuery(queryable.Object, spec), Times.Once);
        }
    }

    public class GetSingleAsyncSpecification : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GetSingleAsyncSpecification(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
        
            var queryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
            var retQueryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
        
            setMock.Setup(x => x.AsQueryable())
                .Returns(queryable.Object);

            var spec = new TestSpecification(test.Id);

            var evaluator = _repoFixture.GetEvaluatorMock();
            evaluator.Setup(x => x.GetQuery(queryable.Object, spec, false)).Returns(retQueryable.Object);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.GetSingleAsync(spec, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().NotBeNull().And.Be(test);
            else
                result.Should().BeNull();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.AsQueryable(), Times.Once);
            evaluator.Verify(x =>  x.GetQuery(queryable.Object, spec, false), Times.Once);
        }
    }
    
    public class GetSingleAsyncGenericSpecification : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GetSingleAsyncGenericSpecification(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task CallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
            var test1 = new TestEntityOffset();
        
            var queryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
            var retQueryable = (id is null ? Array.Empty<TestEntityOffset>() : new [] { test1 }).AsQueryable().BuildMockDbSet();
        
            setMock.Setup(x => x.AsQueryable())
                .Returns(queryable.Object);

            var spec = new TestSpecificationTransform(test.Id);

            var evaluator = _repoFixture.GetEvaluatorMock();
            evaluator.Setup(x => x.GetQuery(queryable.Object, spec)).Returns(retQueryable.Object);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.GetSingleAsync(spec, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().NotBeNull().And.Be(test1);
            else
                result.Should().BeNull();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.AsQueryable(), Times.Once);
            evaluator.Verify(x =>  x.GetQuery(queryable.Object, spec), Times.Once);
        }
    }

    public class GetBySpecAsync : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GetBySpecAsync(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
        
            var queryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();

            var ret = id is null ? Array.Empty<TestEntity>() : new[] { test };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            setMock.Setup(x => x.AsQueryable())
                .Returns(queryable.Object);

            var spec = new TestSpecification(test.Id);

            var evaluator = _repoFixture.GetEvaluatorMock();
            evaluator.Setup(x => x.GetQuery(queryable.Object, spec, false)).Returns(retQueryable.Object);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.GetBySpecAsync(spec, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().NotBeNull().And.Equal(ret);
            else
                result.Should().BeEmpty();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.AsQueryable(), Times.Once);
            evaluator.Verify(x =>  x.GetQuery(queryable.Object, spec, false), Times.Once);
        }
    }

    public class GetBySpecAsyncGeneric : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GetBySpecAsyncGeneric(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
            var test1 = new TestEntityOffset();
        
            var queryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
            var ret = id is null ? Array.Empty<TestEntityOffset>() : new[] { test1 };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            setMock.Setup(x => x.AsQueryable())
                .Returns(queryable.Object);

            var spec = new TestSpecificationTransform(test.Id);

            var evaluator = _repoFixture.GetEvaluatorMock();
            evaluator.Setup(x => x.GetQuery(queryable.Object, spec)).Returns(retQueryable.Object);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.GetBySpecAsync(spec, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().NotBeNull().And.Equal(ret);
            else
                result.Should().BeEmpty();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.AsQueryable(), Times.Once);
            evaluator.Verify(x =>  x.GetQuery(queryable.Object, spec), Times.Once);
        }
    }

    public class GetAsyncSpec : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GetAsyncSpec(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
        
            var queryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
            var ret = id is null ? Array.Empty<TestEntity>() : new[] { test };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            setMock.Setup(x => x.AsQueryable())
                .Returns(queryable.Object);

            var spec = new TestSpecification(test.Id);

            var evaluator = _repoFixture.GetEvaluatorMock();
            evaluator.Setup(x => x.GetQuery(queryable.Object, spec, false)).Returns(retQueryable.Object);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.GetAsync(spec, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().NotBeNull().And.Equal(ret);
            else
                result.Should().BeEmpty();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.AsQueryable(), Times.Once);
            evaluator.Verify(x =>  x.GetQuery(queryable.Object, spec, false), Times.Once);
        }
    }

    public class GetAsyncGenericSpec : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GetAsyncGenericSpec(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
     
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
            var test1 = new TestEntityOffset();
        
            var queryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
            var ret = id is null ? Array.Empty<TestEntityOffset>() : new[] { test1 };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            setMock.Setup(x => x.AsQueryable())
                .Returns(queryable.Object);

            var spec = new TestSpecificationTransform(test.Id);

            var evaluator = _repoFixture.GetEvaluatorMock();
            evaluator.Setup(x => x.GetQuery(queryable.Object, spec)).Returns(retQueryable.Object);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.GetAsync(spec, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().NotBeNull().And.Equal(ret);
            else
                result.Should().BeEmpty();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.AsQueryable(), Times.Once);
            evaluator.Verify(x =>  x.GetQuery(queryable.Object, spec), Times.Once);
        }
    }

    public class AsAsyncEnumerableSpec : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public AsAsyncEnumerableSpec(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
        
            var queryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
            var ret = id is null ? Array.Empty<TestEntity>() : new[] { test };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            setMock.Setup(x => x.AsQueryable())
                .Returns(queryable.Object);

            var spec = new TestSpecification(test.Id);

            var evaluator = _repoFixture.GetEvaluatorMock();
            evaluator.Setup(x => x.GetQuery(queryable.Object, spec, true)).Returns(retQueryable.Object);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = repo.AsAsyncEnumerable(spec);

            var res = new List<TestEntity>();
            await foreach (var ent in result)
            {
                res.Add(ent);
            }

            // Assert
            if (id is not null)
                res.Should().NotBeNull();
            else
                res.Should().BeEmpty();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.AsQueryable(), Times.Once);
            evaluator.Verify(x =>  x.GetQuery(queryable.Object, spec, true), Times.Once);
        }
    }

    public class AsAsyncEnumerableFunc : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public AsAsyncEnumerableFunc(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var test = new TestEntity();
        
            var ret = id is null ? Array.Empty<TestEntity>() : new[] { test };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            ctx.Setup(x => x.Set<TestEntity>()).Returns(retQueryable.Object);
        
            var evaluator = _repoFixture.GetEvaluatorMock();
        
            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = repo.AsAsyncEnumerable(x => x.Id == test.Id);
            var res = new List<TestEntity>();
            await foreach (var ent in result)
            {
                res.Add(ent);
            }

            // Assert
            if (id is not null)
                res.Should().NotBeNull();
            else
                res.Should().BeEmpty();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
        }
    }

    public class AsAsyncEnumerable : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public AsAsyncEnumerable(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
        
            var test = new TestEntity();
        
            var ret = id is null ? Array.Empty<TestEntity>() : new[] { test };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            ctx.Setup(x => x.Set<TestEntity>()).Returns(retQueryable.Object);
        
            var evaluator = _repoFixture.GetEvaluatorMock();

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = repo.AsAsyncEnumerable();
            var res = new List<TestEntity>();
            await foreach (var ent in result)
            {
                res.Add(ent);
            }

            // Assert
            if (id is not null)
                res.Should().NotBeNull();
            else
                res.Should().BeEmpty();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
        }
    }

    public class GetAllAsync : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GetAllAsync(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();

            var test = new TestEntity();
            var ret = id is null ? Array.Empty<TestEntity>() : new[] { test };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            ctx.Setup(x => x.Set<TestEntity>()).Returns(retQueryable.Object);
        
            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, _repoFixture.GetEvaluatorMock().Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.GetAllAsync(CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().NotBeNull().And.Equal(ret);
            else
                result.Should().BeEmpty();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
        }
    }

    public class GetAllAsyncSpec : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public GetAllAsyncSpec(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
            var test1 = new TestEntityOffset();
        
            var queryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
            var ret = id is null ? Array.Empty<TestEntityOffset>() : new[] { test1 };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            setMock.Setup(x => x.AsQueryable())
                .Returns(queryable.Object);
        
            var evaluator = _repoFixture.GetEvaluatorMock();
            evaluator.Setup(x => x.GetQuery(queryable.Object, It.IsAny<ISpecification<TestEntity,TestEntityOffset>>())).Returns(retQueryable.Object);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.GetAllAsync<TestEntityOffset>(CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().NotBeNull().And.Equal(ret);
            else
                result.Should().BeEmpty();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.AsQueryable(), Times.Once);
            evaluator.Verify(x =>  x.GetQuery(queryable.Object, It.IsAny<ISpecification<TestEntity,TestEntityOffset>>()), Times.Once);
        }
    }

    public class AnyAsyncFunc : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public AnyAsyncFunc(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();

            var test = new TestEntity();
            var ret = id is null ? Array.Empty<TestEntity>() : new[] { test };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            ctx.Setup(x => x.Set<TestEntity>()).Returns(retQueryable.Object);
        
            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, _repoFixture.GetEvaluatorMock().Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.AnyAsync(x => x.Id == test.Id, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().BeTrue();
            else
                result.Should().BeFalse();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
        }
    }

    public class AnyAsyncSpec : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public AnyAsyncSpec(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
            var test1 = new TestEntity();
        
            var queryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
            var ret = id is null ? Array.Empty<TestEntity>() : new[] { test1 };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            setMock.Setup(x => x.AsQueryable())
                .Returns(queryable.Object);

            var spec = new TestSpecification(test.Id);
        
            var evaluator = _repoFixture.GetEvaluatorMock();
            evaluator.Setup(x => x.GetQuery(queryable.Object, spec, false)).Returns(retQueryable.Object);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.AnyAsync(spec, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().BeTrue();
            else
                result.Should().BeFalse();

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.AsQueryable(), Times.Once);
            evaluator.Verify(x =>  x.GetQuery(queryable.Object, spec, false), Times.Once);
        }
    }

    public class LongCountAsyncFunc : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public LongCountAsyncFunc(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();

            var test = new TestEntity();
            var ret = id is null ? Array.Empty<TestEntity>() : new[] { test };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            ctx.Setup(x => x.Set<TestEntity>()).Returns(retQueryable.Object);
        
            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, _repoFixture.GetEvaluatorMock().Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.LongCountAsync(x => x.Id == test.Id, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().Be(1);
            else
                result.Should().Be(0);

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
        }
    }

    public class LongCountAsyncSpec : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public LongCountAsyncSpec(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();
            var setMock = new Mock<DbSet<TestEntity>>();

            ctx.Setup(x => x.Set<TestEntity>()).Returns(setMock.Object);

            var test = new TestEntity();
            var test1 = new TestEntity();
        
            var queryable = (id is null ? Array.Empty<TestEntity>() : new [] { test }).AsQueryable().BuildMockDbSet();
            var ret = id is null ? Array.Empty<TestEntity>() : new[] { test1 };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            setMock.Setup(x => x.AsQueryable())
                .Returns(queryable.Object);

            var spec = new TestSpecification(test.Id);
        
            var evaluator = _repoFixture.GetEvaluatorMock();
            evaluator.Setup(x => x.GetQuery(queryable.Object, spec, false)).Returns(retQueryable.Object);

            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, evaluator.Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.LongCountAsync(spec, CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().Be(1);
            else
                result.Should().Be(0);

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
            setMock.Verify(x => x.AsQueryable(), Times.Once);
            evaluator.Verify(x =>  x.GetQuery(queryable.Object, spec, false), Times.Once);
        }
    }

    public class LongCountAsync : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
    {
        private readonly ContextFixture _ctxFixture;
        private readonly RepositoryFixture _repoFixture;

        public LongCountAsync(ContextFixture ctxFixture, RepositoryFixture repoFixture)
        {
            _ctxFixture = ctxFixture;
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task ShouldCallCorrectMethodsAndReturnResult(int? id)
        {
            // Arrange
            var ctx = _ctxFixture.GetEfDbContextMock();

            var test = new TestEntity();
            var ret = id is null ? Array.Empty<TestEntity>() : new[] { test };
            var retQueryable = ret.AsQueryable().BuildMockDbSet();
        
            ctx.Setup(x => x.Set<TestEntity>()).Returns(retQueryable.Object);
        
            var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, _repoFixture.GetEvaluatorMock().Object,
                _repoFixture.GetMapperMock().Object, _repoFixture.GetGridifyMock().Object);

            // Act
            var result = await repo.LongCountAsync(CancellationToken.None);

            // Assert
            if (id is not null)
                result.Should().Be(1);
            else
                result.Should().Be(0);

            ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
        }
    }
}
