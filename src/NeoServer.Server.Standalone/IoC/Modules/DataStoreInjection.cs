using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.DataStore;

namespace NeoServer.Server.Standalone.IoC.Modules
{
    public static class DataStoreInjection
    {
        public static ContainerBuilder AddDataStores(this ContainerBuilder builder)
        {
            // var dataStores = AppDomain
            //     .CurrentDomain
            //     .GetAssemblies()
            //     .SelectMany(x =>
            //         x.GetTypes())
            //     .Where(x =>
            //         typeof(IDataStore).IsAssignableFrom(x) &&
            //         x.IsClass);

            builder.RegisterType<ItemTypeStore>()
                .As<ItemTypeStore>()
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

            return builder;
        }
    }
}