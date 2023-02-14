using Autofac;

namespace NeoServer.Server.Helpers;

public static class IoC
{
    private static IContainer _container;

    public static void Initialize(IContainer container)
    {
        _container = container;
    }

    public static T GetInstance<T>()
    {
        return _container.Resolve<T>();
    }
}