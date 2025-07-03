using System.Collections.Concurrent;
using System.Reflection;
using DataExplorer.Abstractions.Repositories;
using DataExplorer.EfCore.Abstractions;
using DataExplorer.EfCore.Abstractions.Repositories;
using DataExplorer.EfCore.Repositories;
using DataExplorer.Tests.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace DataExplorer.EfCore.Tests.Unit;

public class UnitOfWorkTests
{
    public class UseExplicitTransactionAsync : IClassFixture<ContextFixture>, IClassFixture<UnitOfWorkFixture>
    {
        private readonly UnitOfWorkFixture _uofFixture;
        private readonly ContextFixture _contextFixture;

        public UseExplicitTransactionAsync(ContextFixture contextFixture, UnitOfWorkFixture uofFixture)
        {
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
                _uofFixture.GetIMapperMock().Object, _uofFixture.GetOptionsMock(), _uofFixture.GetIEfDataExplorerTypeCacheMock().Object,
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

    // TODO CLEANUP AND FINISH
    public class GetRepository : IClassFixture<ContextFixture>, IClassFixture<RepositoryFixture>,
        IClassFixture<UnitOfWorkFixture>
    {
        private readonly UnitOfWorkFixture _uofFixture;
        private readonly ContextFixture _contextFixture;
        private readonly RepositoryFixture _repositoryFixture;

        public GetRepository(UnitOfWorkFixture uofFixture, ContextFixture contextFixture, RepositoryFixture repositoryFixture)
        {
            _uofFixture = uofFixture;
            _contextFixture = contextFixture;
            _repositoryFixture = repositoryFixture;
        }

        [Fact]
        public void ShouldThrowWhenDisposed()
        {
            // Arrange
            var ctx = _contextFixture.GetTextContextMock();
            var unitOfWork = new UnitOfWork<ITestContext>(ctx.Object, _uofFixture.GetISpecificationEvaluatorMock().Object,
                _uofFixture.GetIMapperMock().Object, _uofFixture.GetOptionsMock(), _uofFixture.GetIEfDataExplorerTypeCacheMock().Object,
                _uofFixture.GetICachedInstanceFactoryMock().Object);
            unitOfWork.Dispose();
            
            // Act
            Action act = () => unitOfWork.GetRepository<IRepository<TestEntity>>();
            
            // Assert
            act.Should().Throw<ObjectDisposedException>();
        }
        
        [Fact]
        public void ShouldThrowInvalidOperationExceptionWhenTypeIsNotInterfaceOrGeneric()
        {
            // Arrange
            var ctx = _contextFixture.GetTextContextMock();
            var unitOfWork = new UnitOfWork<ITestContext>(ctx.Object, _uofFixture.GetISpecificationEvaluatorMock().Object,
                _uofFixture.GetIMapperMock().Object, _uofFixture.GetOptionsMock(), _uofFixture.GetIEfDataExplorerTypeCacheMock().Object,
                _uofFixture.GetICachedInstanceFactoryMock().Object);
            
            // Act
            Action act = () => unitOfWork.GetRepository<Repository<TestEntity>>();
            
            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("You can only retrieve types*");
        }
        
        [Fact]
        public void ShouldThrowInvalidOperationExceptionWhenTypeIsNotInCache()
        {
            // Arrange
            var ctx = _contextFixture.GetTextContextMock();
            var unitOfWork = new UnitOfWork<ITestContext>(ctx.Object, _uofFixture.GetISpecificationEvaluatorMock().Object,
                _uofFixture.GetIMapperMock().Object, _uofFixture.GetOptionsMock(), _uofFixture.GetIEfDataExplorerTypeCacheMock().Object,
                _uofFixture.GetICachedInstanceFactoryMock().Object);
            
            // Act
            Action act = () => unitOfWork.GetRepository<IRepository<TestEntity>>();
            
            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("Unknown repository type*");
        }
        
        [Fact]
        public void ShouldCreateAndCacheRepositoryInstance()
        {
            // Arrange
            var ctx = _contextFixture.GetTextContextMock();
            var factoryMock = _uofFixture.GetICachedInstanceFactoryMock();
            var repoImplType = typeof(Repository<TestEntity>);
            var repoInterfaceType = typeof(IRepository<TestEntity>);
            
            var repo = _repositoryFixture.GetRepository<TestEntity>(ctx.Object, _uofFixture.GetISpecificationEvaluatorMock().Object,
                _uofFixture.GetIMapperMock().Object);

            var cacheMock = _uofFixture.GetIEfDataExplorerTypeCacheMock();

            var repoInf = new DataExplorerRepoInfo(repoImplType, repoInterfaceType, typeof(long), true, true);
            var repoROInf = new DataExplorerRepoInfo(typeof(ReadOnlyRepository<TestEntity>),
                typeof(IReadOnlyRepository<TestEntity>), typeof(long), false, true);
            var genericRepoInf = new DataExplorerRepoInfo(typeof(Repository<TestEntity, long>),
                typeof(IRepository<TestEntity, long>), typeof(long), true, true);
            var genericRepoROInf = new DataExplorerRepoInfo(typeof(ReadOnlyRepository<TestEntity, long>),
                typeof(IReadOnlyRepository<TestEntity, long>), typeof(long), false, true);
            
            var entityInfo = new DataExplorerEntityInfo(typeof(TestEntity), typeof(TestEntity).GetInterfaces(),
                typeof(long), genericRepoInf, genericRepoROInf, repoInf, repoROInf, false, false, true);
            
            repoInf.SetEntityInfo(entityInfo);
            repoROInf.SetEntityInfo(entityInfo);
            genericRepoInf.SetEntityInfo(entityInfo);
            genericRepoROInf.SetEntityInfo(entityInfo);
            
            cacheMock.Setup(x => x.TryGetRepoInfo(It.Is<Type>(t => t == repoInterfaceType), out repoInf))
                .Returns(true);
            
            factoryMock.Setup(x => x.CreateInstance(It.Is<Type>(t => t == repoImplType), It.IsAny<object>(), 
                    It.IsAny<object>(), It.IsAny<object>()))
                .Returns(repo);
            
            var unitOfWork = new UnitOfWork<ITestContext>(ctx.Object, _uofFixture.GetISpecificationEvaluatorMock().Object,
                _uofFixture.GetIMapperMock().Object, _uofFixture.GetOptionsMock(), 
                cacheMock.Object, factoryMock.Object);

            // ReSharper disable once UseNameOfInsteadOfTypeOf
            var key = new RepositoryEntryKey(typeof(TestEntity).FullName ?? typeof(TestEntity).Name);
            
            // Act
            var resultRepo = unitOfWork.GetRepository<IRepository<TestEntity>>();
            
            // Assert
            resultRepo.Should().BeSameAs(repo);
            var repos = GetRepositoriesFromUof(unitOfWork);
            repos.Should().NotBeNull();
            repos.Should().ContainSingle();
            repos.Should().Contain(x => x.Key.Equals(key) && ReferenceEquals(x.Value.RepoInfo, repoInf) && ReferenceEquals(x.Value.LazyRepo.Value, repo));
        }
        
        [Fact]
        public void ShouldReturnExistingRepositoryInstance()
        {
            // Arrange
            var ctx = _contextFixture.GetTextContextMock();
            var factoryMock = _uofFixture.GetICachedInstanceFactoryMock();
            var repoImplType = typeof(Repository<TestEntity>);
            var repoInterfaceType = typeof(IRepository<TestEntity>);
            
            var repoRO = _repositoryFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, _uofFixture.GetISpecificationEvaluatorMock().Object,
                _uofFixture.GetIMapperMock().Object);
            
            var repo = _repositoryFixture.GetRepository<TestEntity>(ctx.Object, _uofFixture.GetISpecificationEvaluatorMock().Object,
                _uofFixture.GetIMapperMock().Object);

            var cacheMock = _uofFixture.GetIEfDataExplorerTypeCacheMock();

            var repoROInterface = typeof(IReadOnlyRepository<TestEntity>);
            
            var repoInf = new DataExplorerRepoInfo(repoImplType, repoInterfaceType, typeof(long), true, true);
            var repoROInf = new DataExplorerRepoInfo(typeof(ReadOnlyRepository<TestEntity>), repoROInterface, typeof(long), false, true);
            var genericRepoInf = new DataExplorerRepoInfo(typeof(Repository<TestEntity,long>), typeof(IRepository<TestEntity,long>), typeof(long), true, true);
            var genericRepoROInf = new DataExplorerRepoInfo(typeof(ReadOnlyRepository<TestEntity,long>), typeof(IReadOnlyRepository<TestEntity,long>), typeof(long), false, true);
            
            var entityInfo = new DataExplorerEntityInfo(typeof(TestEntity), typeof(TestEntity).GetInterfaces(),
                typeof(long), genericRepoInf, genericRepoROInf, repoInf, repoROInf, false, false, true);
            
            repoInf.SetEntityInfo(entityInfo);
            repoROInf.SetEntityInfo(entityInfo);
            genericRepoInf.SetEntityInfo(entityInfo);
            genericRepoROInf.SetEntityInfo(entityInfo);
            
            cacheMock.Setup(x => x.TryGetRepoInfo(It.Is<Type>(t => t == repoInterfaceType), out repoInf))
                .Returns(true);
            cacheMock.Setup(x => x.TryGetRepoInfo(It.Is<Type>(t => t == repoROInterface), out repoROInf))
                .Returns(true);
            
            var unitOfWork = new UnitOfWork<ITestContext>(ctx.Object, _uofFixture.GetISpecificationEvaluatorMock().Object,
                _uofFixture.GetIMapperMock().Object, _uofFixture.GetOptionsMock(), 
                cacheMock.Object, factoryMock.Object);

            // ReSharper disable once UseNameOfInsteadOfTypeOf
            var key = new RepositoryEntryKey(typeof(TestEntity).FullName ?? typeof(TestEntity).Name);
            
            var repos = InitializeAndGetRepositoriesFromUof(unitOfWork);
            repos!.TryAdd(key, new RepositoryEntry(repoInf, new Lazy<IBaseRepository>(() => repo)));

            if (repos.Count != 1)
                throw new InvalidOperationException();
            
            // Act
            var resultRepo = unitOfWork.GetRepository<IReadOnlyRepository<TestEntity>>();
            
            // Assert
            resultRepo.Should().BeSameAs(repo);
            repos = GetRepositoriesFromUof(unitOfWork);
            repos.Should().NotBeNull();
            repos.Should().ContainSingle();
            repos.Should().Contain(x => x.Key.Equals(key) && ReferenceEquals(x.Value.RepoInfo, repoInf) && ReferenceEquals(x.Value.LazyRepo.Value, repo));
        }
        
        [Fact]
        public void ShouldCacheAndReplaceRepositoryInstance()
        {
            // Arrange
            var ctx = _contextFixture.GetTextContextMock();
            var factoryMock = _uofFixture.GetICachedInstanceFactoryMock();
            var repoImplType = typeof(Repository<TestEntity>);
            var repoInterfaceType = typeof(IRepository<TestEntity>);
            
            var repoRO = _repositoryFixture.GetReadOnlyRepository<TestEntity>(ctx.Object, _uofFixture.GetISpecificationEvaluatorMock().Object,
                _uofFixture.GetIMapperMock().Object);
            
            var repo = _repositoryFixture.GetRepository<TestEntity>(ctx.Object, _uofFixture.GetISpecificationEvaluatorMock().Object,
                _uofFixture.GetIMapperMock().Object);

            var cacheMock = _uofFixture.GetIEfDataExplorerTypeCacheMock();

            var repoROInterface = typeof(IReadOnlyRepository<TestEntity>);
            var repoROImpl = typeof(ReadOnlyRepository<TestEntity>);
            
            var repoInf = new DataExplorerRepoInfo(repoImplType, repoInterfaceType, typeof(long), true, true);
            var repoROInf = new DataExplorerRepoInfo(repoROImpl, repoROInterface, typeof(long), false, true);
            var genericRepoInf = new DataExplorerRepoInfo(typeof(Repository<TestEntity,long>), typeof(IRepository<TestEntity,long>), typeof(long), true, true);
            var genericRepoROInf = new DataExplorerRepoInfo(typeof(ReadOnlyRepository<TestEntity,long>), typeof(IReadOnlyRepository<TestEntity,long>), typeof(long), false, true);
            
            var entityInfo = new DataExplorerEntityInfo(typeof(TestEntity), typeof(TestEntity).GetInterfaces(),
                typeof(long), genericRepoInf, genericRepoROInf, repoInf, repoROInf, false, false, true);
            
            repoInf.SetEntityInfo(entityInfo);
            repoROInf.SetEntityInfo(entityInfo);
            genericRepoInf.SetEntityInfo(entityInfo);
            genericRepoROInf.SetEntityInfo(entityInfo);
            
            cacheMock.Setup(x => x.TryGetRepoInfo(It.Is<Type>(t => t == repoInterfaceType), out repoInf))
                .Returns(true);
            cacheMock.Setup(x => x.TryGetRepoInfo(It.Is<Type>(t => t == repoROInterface), out repoROInf))
                .Returns(true);
            
            factoryMock.Setup(x => x.CreateInstance(It.Is<Type>(t => t == repoImplType), It.IsAny<object>(), 
                    It.IsAny<object>(), It.IsAny<object>()))
                .Returns(repo);
            
            var unitOfWork = new UnitOfWork<ITestContext>(ctx.Object, _uofFixture.GetISpecificationEvaluatorMock().Object,
                _uofFixture.GetIMapperMock().Object, _uofFixture.GetOptionsMock(), 
                cacheMock.Object, factoryMock.Object);

            // ReSharper disable once UseNameOfInsteadOfTypeOf
            var key = new RepositoryEntryKey(typeof(TestEntity).FullName ?? typeof(TestEntity).Name);
            
            var repos = InitializeAndGetRepositoriesFromUof(unitOfWork);
            repos!.TryAdd(key, new RepositoryEntry(repoROInf, new Lazy<IBaseRepository>(() => repoRO)));

            if (repos.Count != 1)
                throw new InvalidOperationException();
            
            // Act
            var resultRepo = unitOfWork.GetRepository<IRepository<TestEntity>>();
            
            // Assert
            resultRepo.Should().NotBeSameAs(repoRO);
            repos = GetRepositoriesFromUof(unitOfWork);
            repos.Should().NotBeNull();
            repos.Should().ContainSingle();
            repos.Should().Contain(x => x.Key.Equals(key) && ReferenceEquals(x.Value.RepoInfo, repoInf) && ReferenceEquals(x.Value.LazyRepo.Value, repo));
        }

        private static ConcurrentDictionary<RepositoryEntryKey, RepositoryEntry>? GetRepositoriesFromUof(
            IUnitOfWork<ITestContext> uof)
            => (ConcurrentDictionary<RepositoryEntryKey, RepositoryEntry>?)RepoCacheField.GetValue(uof);

        private static ConcurrentDictionary<RepositoryEntryKey, RepositoryEntry>? InitializeAndGetRepositoriesFromUof(
            IUnitOfWork<ITestContext> uof)
        {
            RepoCacheField.SetValue(uof, new ConcurrentDictionary<RepositoryEntryKey, RepositoryEntry>());
            return (ConcurrentDictionary<RepositoryEntryKey, RepositoryEntry>?)RepoCacheField.GetValue(uof);
        }

        private static FieldInfo RepoCacheField => typeof(UnitOfWork<ITestContext>).GetField("_repositories", BindingFlags.Instance | BindingFlags.NonPublic)!;
    }
}
