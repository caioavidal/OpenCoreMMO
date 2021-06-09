using Autofac;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creature;
using NeoServer.Game.World.Spawns;
using NeoServer.Server.Contracts;
using NeoServer.Server.Instances;

namespace NeoServer.Server.Standalone.IoC
{
    public static class ManagerInjection
    {
        public static ContainerBuilder AddManagers(this ContainerBuilder builder)
        {
            builder.RegisterType<GameServer>().As<IGameServer>().SingleInstance();
            builder.RegisterType<GameCreatureManager>().As<IGameCreatureManager>().SingleInstance();
            builder.RegisterType<DecayableItemManager>().SingleInstance();

            builder.RegisterType<MonsterDataManager>().As<IMonsterDataManager>().SingleInstance();
            builder.RegisterType<SpawnManager>().SingleInstance();
            return builder;
        }
    }
}