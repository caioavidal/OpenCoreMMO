using Autofac;
using NeoServer.Game.DataStore;

namespace NeoServer.Server.Standalone.IoC.Modules
{
    public static class DataStoreInjection
    {
        public static ContainerBuilder AddDataStores(this ContainerBuilder builder)
        {
            builder.RegisterType<ItemTypeStore>().As<ItemTypeStore>()
                .SingleInstance();

            return builder;
        }
    }
}