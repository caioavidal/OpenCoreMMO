using Microsoft.Extensions.DependencyInjection;

namespace NeoServer.BuildingBlocks.Application;

public static class IoC
{
    private static IServiceProvider _container;

    public static void Initialize(IServiceProvider container)
    {
        _container = container;
    }

    public static T GetInstance<T>() where T : class
    {
        return _container.GetService<T>();
    }
}