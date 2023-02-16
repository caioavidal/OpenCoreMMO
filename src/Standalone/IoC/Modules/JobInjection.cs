using Autofac;
using NeoServer.Server.Jobs.Channels;
using NeoServer.Server.Jobs.Creatures;
using NeoServer.Server.Jobs.Items;
using NeoServer.Server.Jobs.Persistence;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class JobInjection
{
    public static ContainerBuilder AddJobs(this ContainerBuilder builder)
    {
        //todo: inherit these jobs from interface and register by implementation
        builder.RegisterType<GameCreatureJob>().SingleInstance();
        builder.RegisterType<GameItemJob>().SingleInstance();
        builder.RegisterType<GameChatChannelJob>().SingleInstance();
        builder.RegisterType<PlayerPersistenceJob>().SingleInstance();
        return builder;
    }
}