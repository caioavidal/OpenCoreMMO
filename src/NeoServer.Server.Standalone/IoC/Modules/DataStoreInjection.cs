using Autofac;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.DataStore;

namespace NeoServer.Server.Standalone.IoC.Modules
{
    public static class DataStoreInjection
    {
        public static ContainerBuilder AddDataStores(this ContainerBuilder builder)
        {
            builder.RegisterType<ItemTypeStore>()
                .As<ItemTypeStore>()
                .SingleInstance();
            
            builder.RegisterType<ChatChannelStore>()
                .As<IChatChannelStore>()
                .SingleInstance();
            
            builder.RegisterType<GuildStore>()
                .As<IGuildStore>()
                .SingleInstance();

            return builder;
        }
    }
}