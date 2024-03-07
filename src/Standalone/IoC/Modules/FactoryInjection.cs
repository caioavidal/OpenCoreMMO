using Microsoft.Extensions.DependencyInjection;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Game.Chat.Channels;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Creature.Factories;
using NeoServer.Game.Item.Factories;
using NeoServer.Game.Item.Factories.AttributeFactory;
using NeoServer.Game.World.Factories;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class FactoryInjection
{
    public static IServiceCollection AddFactories(this IServiceCollection builder)
    {
        builder.AddSingleton<IItemFactory, ItemFactory>();

        builder.AddSingleton<DefenseEquipmentFactory>();
        builder.AddSingleton<WeaponFactory>();
        builder.AddSingleton<ContainerFactory>();
        builder.AddSingleton<GroundFactory>();
        builder.AddSingleton<RuneFactory>();
        builder.AddSingleton<CumulativeFactory>();
        builder.AddSingleton<GenericItemFactory>();

        builder.AddSingleton<ProtectionFactory>();
        builder.AddSingleton<DecayableFactory>();
        builder.AddSingleton<SkillBonusFactory>();
        builder.AddSingleton<ChargeableFactory>();

        builder.AddSingleton<ChatChannelFactory>();

        builder.AddSingleton<ILiquidPoolFactory, LiquidPoolFactory>();

        builder.AddSingleton<ICreatureFactory, CreatureFactory>();
        builder.AddSingleton<IMonsterFactory, MonsterFactory>();
        builder.AddSingleton<INpcFactory, NpcFactory>();
        builder.AddSingleton<ITileFactory, TileFactory>();

        builder.AddSingleton<PacketHandlerFactory>();

        return builder;
    }
}