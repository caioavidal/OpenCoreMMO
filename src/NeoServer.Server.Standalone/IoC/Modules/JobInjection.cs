using Autofac;
using NeoServer.Server.Jobs.Creatures;
using NeoServer.Server.Jobs.Items;
using NeoServer.Server.Jobs.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Standalone.IoC
{
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
}
