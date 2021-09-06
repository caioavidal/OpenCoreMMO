using System.Collections.Generic;
using Autofac;
using NeoServer.Game.Chats;
using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Creatures.Factories;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.DataStore;
using NeoServer.Game.Items.Factories;
using NeoServer.Networking.Handlers;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Network.Enums;
using Serilog.Core;

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