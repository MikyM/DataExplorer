using DataExplorer.EfCore.Specifications;
using DataExplorer.Tests.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace DataExplorer.EfCore.Tests.Unit;

public class ReadOnlyRepositoryShould : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>
{
    private readonly ContextFixture _ctxFixture;
    private readonly RepositoryFixture _repoFixture;

    public ReadOnlyRepositoryShould(ContextFixture ctxFixture, RepositoryFixture repoFixture)
    {
        _ctxFixture = ctxFixture;
        _repoFixture = repoFixture;
    }

    [Fact]
    public void Return_correct_GridifyMapperProvider()
    {
        // Arrange
        var ctx = _ctxFixture.GetEfDbContextMock();

        var gridify = _repoFixture.GetGridifyMock();
        
        var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx, _repoFixture.GetEvaluatorMock(),
            _repoFixture.GetMapperMock(), gridify);
        
        // Act & Assert
        repo.GridifyMapperProvider.Should().NotBeNull().And.Be(gridify.Object);
    }
    
    [Fact]
    public void Return_correct_Context()
    {
        // Arrange
        var ctx = _ctxFixture.GetEfDbContextMock();

        var gridify = _repoFixture.GetGridifyMock();
        
        var repo = _repoFixture.GetReadOnlyRepository<TestEntity>(ctx, _repoFixture.GetEvaluatorMock(),
            _repoFixture.GetMapperMock(), gridify);
        
        // Act & Assert
        repo.Context.Should().NotBeNull().And.Be(ctx.Object);
    }
    
    [Fact]
    public void Return_correct_SpecificationEvaluator()
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
    
    [Fact]
    public void Return_correct_Set()
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

    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_FindAsync_on_GetAsync_params_and_return_result(int? id)
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

    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_FindAsync_on_GetAsync_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_GetSingleBySpecAsync_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_GetSingleBySpecAsync_generic_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_GetSingleAsync_spec_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_GetSingleAsync_spec_generic_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_GetBySpecAsync_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_GetBySpecAsync_generic_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_GetAsync_spec_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_GetAsync_spec_generic_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public void Call_proper_methods_on_AsAsyncEnumerable_spec_and_return_result(int? id)
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
        var res = result.ToBlockingEnumerable();

        // Assert
        if (id is not null)
            res.Should().NotBeNull();
        else
            res.Should().BeEmpty();

        ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
        setMock.Verify(x => x.AsQueryable(), Times.Once);
        evaluator.Verify(x =>  x.GetQuery(queryable.Object, spec, true), Times.Once);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public void Call_proper_methods_on_AsAsyncEnumerable_func_and_return_result(int? id)
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
        var res = result.ToBlockingEnumerable();

        // Assert
        if (id is not null)
            res.Should().NotBeNull();
        else
            res.Should().BeEmpty();

        ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public void Call_proper_methods_on_AsAsyncEnumerable_and_return_result(int? id)
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
        var res = result.ToBlockingEnumerable();

        // Assert
        if (id is not null)
            res.Should().NotBeNull();
        else
            res.Should().BeEmpty();

        ctx.Verify(x => x.Set<TestEntity>(), Times.Once);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_GetAllAsync_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_GetAllAsync_generic_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_AnyAsync_func_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_AnyAsync_spec_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_LongCount_func_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_LongCountAsync_spec_and_return_result(int? id)
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
    
    [Theory]
    [InlineData(1)]
    [InlineData(null)]
    public async Task Call_proper_methods_on_LongCount_and_return_result(int? id)
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
