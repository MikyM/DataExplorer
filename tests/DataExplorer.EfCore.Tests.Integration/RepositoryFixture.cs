using AutoMapper;
using DataExplorer.EfCore.Abstractions.Repositories;
using DataExplorer.EfCore.Repositories;
using DataExplorer.EfCore.Specifications;
using DataExplorer.EfCore.Specifications.Evaluators;
using DataExplorer.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;

namespace DataExplorer.EfCore.Tests.Integration;

public class RepositoryFixture : IDisposable
{
    private readonly List<TestIntegrationContext> _testContexts = new();
    
    public IRepository<TestEntity> CreateRepository()
    {
        var ctx = new TestIntegrationContext(_dbContextOptions, _config, _timeProvider);
        _testContexts.Add(ctx);
        var repo = new Repository<TestEntity>(ctx, _specificationEvaluator, _mapper);
        return repo;
    }
    
    public IReadOnlyRepository<TestEntity> CreateReadOnlyRepository()
    {
        var ctx = new TestIntegrationContext(_dbContextOptions, _config, _timeProvider);
        _testContexts.Add(ctx);
        var repo = new ReadOnlyRepository<TestEntity>(ctx, _specificationEvaluator, _mapper);
        return repo;
    }

    private readonly IOptions<DataExplorerEfCoreConfiguration> _config;
    private readonly DataExplorerTimeProvider _timeProvider;
    private readonly DbContextOptions<TestIntegrationContext> _dbContextOptions;
    private readonly IEfSpecificationEvaluator _specificationEvaluator;
    private readonly IMapper _mapper;
    
    private readonly PostgreSqlContainer _container;
    
    public RepositoryFixture()
    {
        _config = Options.Create(new DataExplorerEfCoreConfiguration(new ServiceCollection()));
        
        _timeProvider = new DataExplorerTimeProvider.StaticDataExplorerTimeProvider();
        _specificationEvaluator = new SpecificationEvaluator(true);

        var config = new MapperConfiguration(cfg => {
            cfg.CreateMap<DateTime?, DateTimeOffset?>().ConvertUsing(x => x == null ? null : new DateTimeOffset(x.Value));
            cfg.CreateMap<TestEntity,TestEntityOffset>();
        });
        _mapper = config.CreateMapper();

        var port = Random.Shared.Next(5000, 10000);
        
        _container = new PostgreSqlBuilder()
            .WithDatabase("test")
            .WithPassword("test")
            .WithUsername("test")
            .WithImage("postgres:17.5-bookworm")
            .WithPortBinding(port, port)
            .Build();

        Task.Delay(2000).Wait();
        
        _container.StartAsync().Wait();
        
        var optionsBuilder = new DbContextOptionsBuilder<TestIntegrationContext>();
        
#if NET9_0_OR_GREATER
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
#endif
        
        optionsBuilder.UseNpgsql(_container.GetConnectionString());
        _dbContextOptions = optionsBuilder.Options;
        
        var initCtx = new TestIntegrationContext(_dbContextOptions, _config, _timeProvider);
        initCtx.Database.Migrate();
        initCtx.Dispose();
        
        InitEntity();
    }

    private void InitEntity()
    {
        var now = DateTime.Now.ToUniversalTime();
        
        var ent1 = new TestEntity
        {
            Name = "test1",
            Description = "test1",
            CreatedAt = now,
            UpdatedAt = now
        };
        ent1.SetId(1);
        var ent2 = new TestEntity
        {
            Name = "test2",
            Description = "test2",
            CreatedAt = now,
            UpdatedAt = now
        };
        ent2.SetId(2);
        var ent3 = new TestEntity
        {
            Name = "test3",
            Description = "test3",
            CreatedAt = now,
            UpdatedAt = now
        };
        ent3.SetId(3);
        var ent4 = new TestEntity
        {
            Name = "test4",
            Description = "test4",
            CreatedAt = now,
            UpdatedAt = now
        };
        ent4.SetId(4);
        var ent5 = new TestEntity
        {
            Name = "test5",
            Description = "test5",
            CreatedAt = now,
            UpdatedAt = now
        };
        ent5.SetId(5);
        
        var ctx = new TestIntegrationContext(_dbContextOptions, _config, _timeProvider);
        ctx.AddRange(ent1, ent2, ent3, ent4, ent5);
        ctx.SaveChanges();
        ctx.Dispose();
    }

    public void Dispose()
    {
        var last = _testContexts.Last();
        
        foreach (var ctx in _testContexts)
        {
            if (!ReferenceEquals(ctx,last))
            {
                ctx.Dispose();
            }
        }

        last.Database.EnsureDeleted();
        last.Dispose();

        _container.StopAsync().Wait();
        _container.DisposeAsync().AsTask().Wait();
    }
}
