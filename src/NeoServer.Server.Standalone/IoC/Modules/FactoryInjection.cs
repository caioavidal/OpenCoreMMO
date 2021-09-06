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
    public static class FactoryInjection
    {
        public static ContainerBuilder AddFactories(this ContainerBuilder builder)
        {
            builder.RegisterType<AccessoryFactory>().SingleInstance();

            builder.RegisterType<ItemFactory>().As<IItemFactory>().OnActivated(e =>
                {
                    e.Instance.AccessoryFactory = e.Context.Resolve<AccessoryFactory>();
                    e.Instance.ItemEventSubscribers = e.Context.Resolve<IEnumerable<IItemEventSubscriber>>();
                    e.Instance.ItemTypeStore = e.Context.Resolve<ItemTypeStore>();
                })
                .SingleInstance();
            builder.RegisterType<ChatChannelFactory>().OnActivated(e =>
                    e.Instance.ChannelEventSubscribers = e.Context.Resolve<IEnumerable<IChatChannelEventSubscriber>>())
                .SingleInstance();
            builder.RegisterType<LiquidPoolFactory>().As<ILiquidPoolFactory>().SingleInstance();

            builder.RegisterType<CreatureFactory>().As<ICreatureFactory>().SingleInstance();
            builder.RegisterType<MonsterFactory>().As<IMonsterFactory>().SingleInstance();
            builder.RegisterType<NpcFactory>().As<INpcFactory>().SingleInstance();

            builder.RegisterPlayerFactory();
            builder.RegisterIncomingPacketFactory();

            return builder;
        }

        private static void RegisterIncomingPacketFactory(this ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var conn = p.TypedAs<IConnection>();

                var packet = GameIncomingPacketType.PlayerLogOut;

                if (!conn.Disconnected) packet = conn.InMessage.GetIncomingPacketType(conn.IsAuthenticated);

                if (!InputHandlerMap.Data.TryGetValue(packet, out var handlerType))
                    return new NotImplementedPacketHandler(packet, c.Resolve<Logger>());

                if (c.TryResolve(handlerType, out var instance))
                {
                    c.Resolve<Logger>().Debug("{incoming}: {packet}", "Incoming Packet", packet);

                    return (IPacketHandler) instance;
                }

                return new NotImplementedPacketHandler(packet, c.Resolve<Logger>());
            });
        }

        private static void RegisterPlayerFactory(this ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var player = p.TypedAs<Player>();

                return c.Resolve<ICreatureFactory>().CreatePlayer(player);
            });
        }
    }
}