using Microsoft.Extensions.DependencyInjection;
using NeoServer.BuildingBlocks.Application;
using NeoServer.BuildingBlocks.Domain;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Creature.Party;
using NeoServer.Game.Creature.Services;
using NeoServer.Game.Item.Services;
using NeoServer.Game.Item.Services.ItemTransform;
using NeoServer.Game.World.Services;
using NeoServer.Modules.Trading;
using NeoServer.Modules.Trading.TradeExchange;
using NeoServer.PacketHandler.Features.Player.UseItem;
using NeoServer.PacketHandler.Features.Shared;

namespace NeoServer.Server.Standalone.IoC.Modules;

/// <summary>
///     Contains the registration of various game services and operations for the dependency injection container.
/// </summary>
public static class ServiceInjection
{
    /// <summary>
    ///     Registers various game services and operations with the dependency injection container.
    /// </summary>
    /// <param name="builder">The container builder instance.</param>
    /// <returns>The container builder instance with the registered services and operations.</returns>
    public static IServiceCollection AddServices(this IServiceCollection builder)
    {
        //Game services
        builder.AddSingleton<IDealTransaction, DealTransaction>();
        builder.AddSingleton<ICoinTransaction, CoinTransaction>();
        builder.AddSingleton<ISharedExperienceConfiguration, SharedExperienceConfiguration>();
        builder.AddSingleton<IPartyInviteService, PartyInviteService>();
        builder.AddSingleton<ISummonService, SummonService>();
        builder.AddSingleton<IToMapMovementService, ToMapMovementService>();
        builder.AddSingleton<IMapService, MapService>();
        builder.AddSingleton<IMapTool, MapTool>();
        builder.AddSingleton<IPlayerUseService, PlayerUseService>();
        builder.AddSingleton<IItemMovementService, ItemMovementService>();
        builder.AddSingleton<IItemService, ItemService>();
        builder.AddSingleton<IStaticToDynamicTileService, StaticToDynamicTileService>();
        builder.AddSingleton<SafeTradeSystem>();

        builder.AddSingleton<ItemFinder>();
        builder.AddSingleton<WalkToTarget>();

        //Operations
        builder.AddSingleton<TradeItemExchanger>();

        //Items
        builder.AddSingleton<IItemTransformService, ItemTransformService>();
        builder.AddSingleton<IItemRemoveService, ItemRemoveService>();

        //game builders
        builder.RegisterAssemblyTypes<IInspectionTextBuilder>(Container.AssemblyCache);

        //application services
        builder.AddSingleton<IHotkeyService, HotkeyService>();

        return builder;
    }
}