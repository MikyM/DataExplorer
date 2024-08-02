using DataExplorer.EfCore.Specifications;
using DataExplorer.Tests.Shared;
using FluentAssertions;

namespace DataExplorer.EfCore.Tests.Integration.Specifications;

#if NET7_0_OR_GREATER

[CollectionDefinition("ReadOnlyRepositoryTests")]
public class UpdateEvaluatorTests : ICollectionFixture<RepositoryFixture>
{
    [Collection("ReadOnlyRepositoryTests")]
    public class EvaluateShould
    {
        private readonly RepositoryFixture _fixture;

        public EvaluateShould(RepositoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("a", "b", 1)]
        [InlineData("aa", "bb", 5)]
        [InlineData("aaa", "bbb", 11)]
        public async Task NotCauseExceptionsOnMultipleModifies(string name, string description, int version)
        {
            // Arrange
            var entity = TestEntity.Create();
            
            entity.Name = "John";
            entity.Description = "desc";

            var repo = _fixture.CreateRepository();
            
            await repo.AddAsync(entity);

            await repo.Context.SaveChangesAsync();

            var spec = new TestUpdateSpecification(name, description, version, DateTime.Now.ToUniversalTime());

            // Act
            var func = () => repo.ExecuteUpdateAsync(spec);
            
            // Assert
            await func.Should()
                .NotThrowAsync();
        }
        
        [Theory]
        [InlineData("a", "b", 1)]
        [InlineData("aa", "bb", 5)]
        [InlineData("aaa", "bbb", 11)]
        public async Task CauseCorrectUpdates(string name, string description, int version)
        {
            // Arrange
            var entity = TestEntity.Create();
            
            entity.Name = "John";
            entity.Description = "desc";

            var repo = _fixture.CreateRepository();

            await repo.AddAsync(entity);

            await repo.Context.SaveChangesAsync();

            var updatedAt = DateTime.Now.ToUniversalTime();

            var spec = new TestUpdateSpecification(name, description, version, updatedAt);

            // Act
            await repo.ExecuteUpdateAsync(spec);
            
            // Assert
            var get = await repo.GetSingleAsync(new EntityByIdSpecification<TestEntity>(entity.Id));

            get.Should().NotBeNull();
            
            get!.Description.Should().Be(description);

            get!.Name.Should().Be(name);

            get!.UpdatedAt.Should().BeCloseTo(updatedAt, TimeSpan.FromMilliseconds(100));

            get!.Version.Should().Be(version);
        }

        public class TestUpdateSpecification : UpdateSpecification<TestEntity>
        {
            public TestUpdateSpecification(string name, string description, int version, DateTime updatedAt)
            {
                Modify(x => x.SetProperty(p => p.Name, name));
                
                Modify(x => x.SetProperty(p => p.Description, description));
                
                Modify(x => x.SetProperty(p => p.Version, version));
                
                Modify(x => x.SetProperty(p => p.UpdatedAt, updatedAt));
            }
        }
    }
}

#endif
