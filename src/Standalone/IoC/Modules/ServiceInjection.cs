using System;
using Autofac;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Creatures.Party;
using NeoServer.Game.Creatures.Services;
using NeoServer.Game.Items.Services;
using NeoServer.Game.Items.Services.ItemTransform;
using NeoServer.Game.World.Services;
using NeoServer.Server.Commands.Player.UseItem;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class ServiceInjection
{
    public static ContainerBuilder AddServices(this ContainerBuilder builder)
    {
        //Game services
        builder.RegisterType<DealTransaction>().As<IDealTransaction>().SingleInstance();
        builder.RegisterType<CoinTransaction>().As<ICoinTransaction>().SingleInstance();
        builder.RegisterType<SharedExperienceConfiguration>().As<ISharedExperienceConfiguration>().SingleInstance();
        builder.RegisterType<PartyInviteService>().As<IPartyInviteService>().SingleInstance();
        builder.RegisterType<SummonService>().As<ISummonService>().SingleInstance();
        builder.RegisterType<ToMapMovementService>().As<IToMapMovementService>().SingleInstance();
        builder.RegisterType<MapService>().As<IMapService>().SingleInstance();
        builder.RegisterType<MapTool>().As<IMapTool>().SingleInstance();
        builder.RegisterType<PlayerUseService>().As<IPlayerUseService>().SingleInstance();
        builder.RegisterType<ItemMovementService>().As<IItemMovementService>().SingleInstance();
        builder.RegisterType<ItemService>().As<IItemService>().SingleInstance();
        builder.RegisterType<StaticToDynamicTileService>().As<IStaticToDynamicTileService>().SingleInstance();

        //Items
        builder.RegisterType<DecayService>().As<IDecayService>().SingleInstance();
        builder.RegisterType<ItemTransformService>().As<IItemTransformService>().SingleInstance();


        //game builders
        builder.RegisterAssemblyTypes(Container.AssemblyCache).As<IInspectionTextBuilder>()
            .SingleInstance();

        //application services
        builder.RegisterType<HotkeyService>().SingleInstance();

        return builder;
    }
}