using Microsoft.Extensions.DependencyInjection;
using NeoServer.BuildingBlocks.Infrastructure.Data.InMemory;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Modules.Chat.Channel;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class DataStoreInjection
{
    public static IServiceCollection AddDataStores(this IServiceCollection builder)
    {
        builder.AddSingleton<IItemTypeStore, ItemTypeStore>();

        builder.AddSingleton<IChatChannelStore, ChatChannelStore>();

        builder.AddSingleton<IGuildStore, GuildStore>();

        builder.AddSingleton<INpcStore, NpcStore>();

        builder.AddSingleton<IVocationStore, VocationStore>();

        builder.AddSingleton<ICoinTypeStore, CoinTypeStore>();

        builder.AddSingleton<IAreaEffectStore, AreaEffectStore>();

        builder.AddSingleton<IPlayerOutFitStore, PlayerOutFitStore>();

        builder.AddSingleton<IQuestDataStore, QuestDataDataStore>();

        builder.AddSingleton<IItemClientServerIdMapStore, ItemClientServerIdMapStore>();
        builder.AddSingleton<IItemTextWindowStore, ItemTextWindowStore>();

        return builder;
    }
}