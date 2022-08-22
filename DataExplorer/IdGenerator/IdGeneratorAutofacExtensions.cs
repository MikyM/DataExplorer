using Autofac;
using Microsoft.Extensions.Options;
using IdGeneratorOptions = IdGen.IdGeneratorOptions;

namespace DataExplorer.IdGenerator;

/// <summary>
/// Helper class to integrate IdGen with Autofac.
/// </summary>
[PublicAPI]
public static class IdGenAutofacExtensions
{
    /// <summary>
    /// Registers a singleton <see cref="IdGenerator"/> with the given <paramref name="generatorId"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ContainerBuilder"/> to register the singleton <see cref="IdGenerator"/> on.</param>
    /// <param name="generatorId">The generator-id to use for the singleton <see cref="IdGenerator"/>.</param>
    /// <returns>The given <see cref="ContainerBuilder"/> with the registered singleton in it.</returns>
    public static ContainerBuilder AddIdGen(this ContainerBuilder builder, int generatorId)
        => AddIdGen(builder, generatorId, () => IdGeneratorOptions.Default);

    /// <summary>
    /// Registers a singleton <see cref="IdGenerator"/> with the given <paramref name="generatorId"/> and <see cref="IdGeneratorOptions"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ContainerBuilder"/> to register the singleton <see cref="IdGenerator"/> on.</param>
    /// <param name="generatorId">The generator-id to use for the singleton <see cref="IdGenerator"/>.</param>
    /// <param name="options">The <see cref="IdGeneratorOptions"/> for the singleton <see cref="IdGenerator"/>.</param>
    /// <returns>The given <see cref="ContainerBuilder"/> with the registered singleton <see cref="IdGenerator"/> in it.</returns>
    public static ContainerBuilder AddIdGen(this ContainerBuilder builder, int generatorId,
        Func<IdGeneratorOptions> options)
    {
        var opt = options();
        
        builder.RegisterInstance(new IdGen.IdGenerator(generatorId, opt)).As<IdGen.IIdGenerator<long>>()
            .SingleInstance();
        builder.Register(c => (IdGen.IdGenerator)c.Resolve<IdGen.IIdGenerator<long>>()).AsSelf().SingleInstance();

        var iopt = Options.Create(opt);
        
        builder.RegisterInstance(iopt).As<IOptions<IdGeneratorOptions>>().SingleInstance();
        builder.Register(x => x.Resolve<IOptions<IdGeneratorOptions>>().Value).SingleInstance();
        
        builder.RegisterBuildCallback(scope =>
            IdGeneratorFactory.AddFactoryMethod(scope.Resolve<IdGen.IdGenerator>, generatorId));
        
        return builder;
    }
}
