using Microsoft.Extensions.DependencyInjection;
using NeoServer.Server.Jobs.Channels;
using NeoServer.Server.Jobs.Creatures;
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
        builder.AddSingleton<GameChatChannelJob>();
        builder.AddSingleton<PlayerPersistenceJob>();
        return builder;
    }
}