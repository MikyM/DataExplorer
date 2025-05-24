using DataExplorer.Tests.Shared;
using FluentAssertions;

namespace DataExplorer.EfCore.Tests.Integration;

[CollectionDefinition("ReadOnlyRepositoryTests")]
public class ReadOnlyRepositoryTests : ICollectionFixture<RepositoryFixture>
{
    [Collection("ReadOnlyRepositoryTests")]
    public class GetAsyncParams
    {

        private readonly RepositoryFixture _repoFixture;

        public GetAsyncParams(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();

            // Act
            var result = await repo.GetAsync(id);

            // Assert
            if (id == 1)
                result.Should().NotBeNull();
            else
                result.Should().BeNull();
        }
    }
    
    [Collection("ReadOnlyRepositoryTests")]
    public class GetAsync
    {

        private readonly RepositoryFixture _repoFixture;

        public GetAsync(RepositoryFixture repoFixture)
        {
            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();

            // Act
            var result = await repo.GetAsync(new object?[] { id }, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().NotBeNull();
            else
                result.Should().BeNull();
        }
    }
    
    [Collection("ReadOnlyRepositoryTests")]
    public class GetSingleBySpecAsync
    {

        private readonly RepositoryFixture _repoFixture;

        public GetSingleBySpecAsync(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();
            var spec = new TestSpecification(id);
            
            // Act
            var result = await repo.GetSingleBySpecAsync(spec, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().NotBeNull();
            else
                result.Should().BeNull();
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class GetSingleBySpecAsyncGeneric
    {

        private readonly RepositoryFixture _repoFixture;

        public GetSingleBySpecAsyncGeneric(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();
            var spec = new TestSpecificationTransform(id);

            // Act
            var result = await repo.GetSingleBySpecAsync(spec, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().NotBeNull();
            else
                result.Should().BeNull();
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class GetSingleAsyncSpecification
    {

        private readonly RepositoryFixture _repoFixture;

        public GetSingleAsyncSpecification(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();
            var spec = new TestSpecification(id);

            // Act
            var result = await repo.GetSingleAsync(spec, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().NotBeNull();
            else
                result.Should().BeNull();
        }
    }
    
    [Collection("ReadOnlyRepositoryTests")]
    public class GetSingleAsyncGenericSpecification
    {

        private readonly RepositoryFixture _repoFixture;

        public GetSingleAsyncGenericSpecification(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();
            var spec = new TestSpecificationTransform(id);

            // Act
            var result = await repo.GetSingleAsync(spec, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().NotBeNull();
            else
                result.Should().BeNull();
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class GetBySpecAsync
    {

        private readonly RepositoryFixture _repoFixture;

        public GetBySpecAsync(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();
            var spec = new TestSpecification(id);

            // Act
            var result = await repo.GetBySpecAsync(spec, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().NotBeNull().And.HaveCount(1);
            else
                result.Should().BeEmpty();
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class GetBySpecAsyncGeneric
    {

        private readonly RepositoryFixture _repoFixture;

        public GetBySpecAsyncGeneric(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();
            var spec = new TestSpecificationTransform(id);
            
            // Act
            var result = await repo.GetBySpecAsync(spec, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().NotBeNull().And.HaveCount(1);
            else
                result.Should().BeEmpty();
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class GetAsyncSpec
    {

        private readonly RepositoryFixture _repoFixture;

        public GetAsyncSpec(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();
            var spec = new TestSpecification(id);

            // Act
            var result = await repo.GetAsync(spec, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().NotBeNull().And.HaveCount(1);
            else
                result.Should().BeEmpty();
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class GetAsyncGenericSpec
    {

        private readonly RepositoryFixture _repoFixture;

        public GetAsyncGenericSpec(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
     
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();
            var spec = new TestSpecificationTransform(id);

            // Act
            var result = await repo.GetAsync(spec, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().NotBeNull().And.HaveCount(1);
            else
                result.Should().BeEmpty();
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class AsAsyncEnumerableSpec
    {

        private readonly RepositoryFixture _repoFixture;

        public AsAsyncEnumerableSpec(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();
            var spec = new TestSpecification(id);

            // Act
            var result = repo.AsAsyncEnumerable(spec);

            var res = new List<TestEntity>();
            await foreach (var ent in result)
            {
                res.Add(ent);
            }

            // Assert
            if (id == 1)
                res.Should().NotBeNull().And.HaveCount(1);
            else
                res.Should().BeEmpty();
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class AsAsyncEnumerableFunc
    {

        private readonly RepositoryFixture _repoFixture;

        public AsAsyncEnumerableFunc(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();

            // Act
            var result = repo.AsAsyncEnumerable(x => x.Id == id);
            var res = new List<TestEntity>();
            await foreach (var ent in result)
            {
                res.Add(ent);
            }

            // Assert
            if (id == 1)
                res.Should().NotBeNull().And.HaveCount(1);
            else
                res.Should().BeEmpty();
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class AsAsyncEnumerable
    {

        private readonly RepositoryFixture _repoFixture;

        public AsAsyncEnumerable(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Fact]
        public async Task ShouldReturnCorrectResult()
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();

            // Act
            var result = repo.AsAsyncEnumerable();
            var res = new List<TestEntity>();
            await foreach (var ent in result)
            {
                res.Add(ent);
            }

            // Assert
            res.Should().NotBeNull().And.HaveCount(5);
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class GetAllAsync
    {

        private readonly RepositoryFixture _repoFixture;

        public GetAllAsync(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Fact]
        public async Task ShouldReturnCorrectResult()
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();

            // Act
            var result = await repo.GetAllAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull().And.HaveCount(5);
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class GetAllAsyncSpec
    {

        private readonly RepositoryFixture _repoFixture;

        public GetAllAsyncSpec(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Fact]
        public async Task ShouldReturnCorrectResult()
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();

            // Act
            var func = () => repo.GetAllAsync<TestEntityOffset>(CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<InvalidOperationException>();
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class AnyAsyncFunc
    {

        private readonly RepositoryFixture _repoFixture;

        public AnyAsyncFunc(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();

            // Act
            var result = await repo.AnyAsync(x => x.Id == id, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().BeTrue();
            else
                result.Should().BeFalse();
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class AnyAsyncSpec
    {

        private readonly RepositoryFixture _repoFixture;

        public AnyAsyncSpec(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();
            var spec = new TestSpecification(id);

            // Act
            var result = await repo.AnyAsync(spec, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().BeTrue();
            else
                result.Should().BeFalse();
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class LongCountAsyncFunc
    {

        private readonly RepositoryFixture _repoFixture;

        public LongCountAsyncFunc(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();

            // Act
            var result = await repo.LongCountAsync(x => x.Id == id, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().Be(1);
            else
                result.Should().Be(0);
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class LongCountAsyncSpec
    {

        private readonly RepositoryFixture _repoFixture;

        public LongCountAsyncSpec(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(6114452)]
        public async Task ShouldReturnCorrectResult(long id)
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();
            var spec = new TestSpecification(id);

            // Act
            var result = await repo.LongCountAsync(spec, CancellationToken.None);

            // Assert
            if (id == 1)
                result.Should().Be(1);
            else
                result.Should().Be(0);
        }
    }

    [Collection("ReadOnlyRepositoryTests")]
    public class LongCountAsync
    {

        private readonly RepositoryFixture _repoFixture;

        public LongCountAsync(RepositoryFixture repoFixture)
        {

            _repoFixture = repoFixture;
        }
        
        [Fact]
        public async Task ShouldReturnCorrectResult()
        {
            // Arrange
            var repo = _repoFixture.CreateReadOnlyRepository();

            // Act
            var result = await repo.LongCountAsync(CancellationToken.None);

            // Assert
            result.Should().Be(5);
        }
    }
}
