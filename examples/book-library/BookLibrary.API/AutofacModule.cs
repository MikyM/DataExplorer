using Autofac;
using DataExplorer;
using DataExplorer.EfCore;
using DataExplorer.EfCore.Extensions;

namespace BookLibrary.API;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var entityAsm = typeof(Book).Assembly;
        var serviceAsm = typeof(AddBookRequest).Assembly;

        builder.AddDataExplorer(x =>
        {
            x.AddSnowflakeIdGeneration();
            
            x.AddEfCore(opt =>
            {
                opt.EnableIncludeCache = true;
                opt.DateTimeStrategy = DateTimeStrategy.UtcNow;
            }, entityAsm, serviceAsm);
            
            x.AutoMapperConfiguration = mapper =>
            {
                mapper.AllowNullCollections = true;
            };
            
            x.AutoMapperProfileAssembliesAccessor = () => new[] { serviceAsm };
        });
        
        base.Load(builder);
    }
}
