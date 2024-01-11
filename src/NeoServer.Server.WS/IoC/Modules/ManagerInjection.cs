using Autofac;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Creatures.Monster.Managers;
using NeoServer.Game.Systems.Depot;
using NeoServer.Game.World.Models.Spawns;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Managers;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class ManagerInjection
{
    public static ContainerBuilder AddManagers(this ContainerBuilder builder)
    {
        builder.RegisterType<GameServer>().As<IGameServer>().SingleInstance();
        builder.RegisterType<GameCreatureManager>().As<IGameCreatureManager>().SingleInstance();
        builder.RegisterType<DecayableItemManager>().As<IDecayableItemManager>().SingleInstance();


        builder.RegisterType<MonsterDataManager>().As<IMonsterDataManager>().SingleInstance();
        builder.RegisterType<SpawnManager>().SingleInstance();
        builder.RegisterType<DepotManager>().SingleInstance();
        return builder;
    }
}