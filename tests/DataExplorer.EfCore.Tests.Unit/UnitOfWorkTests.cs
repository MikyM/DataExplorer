using DataExplorer.Tests.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace DataExplorer.EfCore.Tests.Unit;

public class UnitOfWorkTests
{
    public class UseExplicitTransactionAsync : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>, IClassFixture<UnitOfWorkFixture>
    {
        private readonly UnitOfWorkFixture _uofFixture;
        private readonly RepositoryFixture _repositoryFixture;
        private readonly ContextFixture _contextFixture;

        public UseExplicitTransactionAsync(RepositoryFixture repositoryFixture, ContextFixture contextFixture, UnitOfWorkFixture uofFixture)
        {
            _repositoryFixture = repositoryFixture;
            _contextFixture = contextFixture;
            _uofFixture = uofFixture;
        }

        [Fact]
        public async Task ShouldBeginNewTransactionWhenNull()
        {
            // Arrange
            
            var trs = new Mock<IDbContextTransaction>();
            var trsObject = trs.Object;
            
            var ctx = _contextFixture.GetTextContextMock();
            
            var facade = new Mock<DatabaseFacade>(ctx.Object);
            facade.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(trsObject);

            ctx.SetupGet(x => x.Database).Returns(facade.Object);

            var unitOfWork = new UnitOfWork<ITestContext>(ctx.Object, _uofFixture.GetISpecificationEvaluatorMock().Object,
                _uofFixture.GetIMapperMock().Object, _uofFixture.GetIOptionsMock().Object,
                _uofFixture.GetIGridifyMapperProviderMock().Object, _uofFixture.GetIEfDataExplorerTypeCacheMock().Object,
                _uofFixture.GetICachedInstanceFactoryMock().Object);

            // Act
            var transaction = await unitOfWork.UseExplicitTransactionAsync();

            // Assert
            ctx.VerifyGet(x => x.Database, Times.Once);
            facade.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            transaction.Should().NotBeNull();
            unitOfWork.Transaction.Should().NotBeNull();
        }
    }
}
