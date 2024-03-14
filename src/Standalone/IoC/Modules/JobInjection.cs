using Microsoft.Extensions.DependencyInjection;
using NeoServer.Application.Features.Chat.Channel;
using NeoServer.Application.Features.Creature.Routines;
using NeoServer.Application.Features.Session.Ping;
using NeoServer.Application.Server.Jobs.Items;
using NeoServer.Application.Server.Jobs.Persistence;

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
        return builder;
    }
}