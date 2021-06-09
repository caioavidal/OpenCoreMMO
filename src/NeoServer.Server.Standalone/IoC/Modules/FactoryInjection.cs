using System.Collections.Generic;
using Autofac;
using NeoServer.Game.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Creatures;
using NeoServer.Game.Items;
using NeoServer.Game.Items.Factories;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Network.Enums;
using NeoServer.Server.Handlers;
using NeoServer.Server.Handlers.Authentication;
using NeoServer.Server.Model.Players;
using Serilog.Core;

namespace NeoServer.Server.Standalone.IoC
{
    public static class FactoryInjection
    {
        public static ContainerBuilder AddFactories(this ContainerBuilder builder)
        {
            builder.RegisterType<ItemFactory>().As<IItemFactory>().OnActivated(e =>
                    e.Instance.ItemEventSubscribers = e.Context.Resolve<IEnumerable<IItemEventSubscriber>>())
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