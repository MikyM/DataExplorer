using DataExplorer.Tests.Shared;

namespace DataExplorer.Extensions.AutoMapper.Integration;

[CollectionDefinition("ReadOnlyRepositoryTests")]
public class ReadOnlyRepositoryTests : ICollectionFixture<RepositoryFixture>
{
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
            var result = await repo.GetAllAsync<TestEntityOffset>(CancellationToken.None);

            // Assert
            result.Should().HaveCount(5);
        }
    }
}
