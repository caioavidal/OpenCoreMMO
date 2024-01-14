using Microsoft.Extensions.DependencyInjection;
using NeoServer.Application.Features.Chat.Channel.Routines;
using NeoServer.Application.Features.Creature.Routines;
using NeoServer.Server.Jobs.Items;
using NeoServer.Server.Jobs.Persistence;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class JobInjection
{
    public static IServiceCollection AddJobs(this IServiceCollection builder)
    {
        //todo: inherit these jobs from interface and register by implementation
        builder.AddSingleton<GameCreatureJob>();
        builder.AddSingleton<GameItemJob>();
        builder.AddSingleton<GameChatChannelRoutine>();
        builder.AddSingleton<PlayerPersistenceJob>();
        return builder;
    }
}