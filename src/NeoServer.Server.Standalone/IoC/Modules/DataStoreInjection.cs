using Autofac;
using NeoServer.Data.InMemory.DataStores;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class DataStoreInjection
{
    public static ContainerBuilder AddDataStores(this ContainerBuilder builder)
    {
        builder.RegisterType<ItemTypeStore>()
            .As<IItemTypeStore>()
            .SingleInstance();

        builder.RegisterType<ChatChannelStore>()
            .As<IChatChannelStore>()
            .SingleInstance();

        builder.RegisterType<GuildStore>()
            .As<IGuildStore>()
            .SingleInstance();

        builder.RegisterType<NpcStore>()
            .As<INpcStore>()
            .SingleInstance();

        builder.RegisterType<VocationStore>()
            .As<IVocationStore>()
            .SingleInstance();

        builder.RegisterType<CoinTypeStore>()
            .As<ICoinTypeStore>()
            .SingleInstance();

        builder.RegisterType<AreaTypeStore>()
            .As<IAreaTypeStore>()
            .SingleInstance();

        return builder;
    }
}