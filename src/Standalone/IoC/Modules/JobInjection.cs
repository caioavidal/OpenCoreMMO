using Microsoft.Extensions.DependencyInjection;
using NeoServer.BuildingBlocks.Application.Server.Jobs.Persistence;
using NeoServer.Modules.Chat.Channel;
using NeoServer.Modules.Combat.AutoAttack;
using NeoServer.Modules.ItemManagement.DecayManagement;
using NeoServer.Modules.Session.Ping;
using NeoServer.PacketHandler.Features.Creature.Routines;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class JobInjection
{
    public static IServiceCollection AddJobs(this IServiceCollection builder)
    {
        //todo: inherit these jobs from interface and register by implementation
        builder.AddSingleton<GameCreatureJob>();
        builder.AddSingleton<DecayRoutine>();
        builder.AddSingleton<GameChatChannelRoutine>();
        builder.AddSingleton<PlayerPersistenceJob>();
        builder.AddSingleton<PingRoutine>();
        builder.AddSingleton<AutoAttackRoutine>();
        return builder;
    }
}