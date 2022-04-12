using Autofac;

namespace NeoServer.Server.Helpers;

public static class Fabric
{
    private static IContainer _container;

    public static void Initialize(IContainer container) => _container = container;

    public static T Return<T>()
    {
        return _container.Resolve<T>();
    }
}