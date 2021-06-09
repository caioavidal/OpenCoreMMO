using Autofac;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Standalone.IoC
{
    public static class EventInjection
    {
        public static ContainerBuilder AddEvents(this ContainerBuilder builder)
        {
            builder.RegisterServerEvents();
            builder.RegisterGameEvents();
            builder.RegisterEventSubscribers();
            builder.RegisterType<EventSubscriber>().SingleInstance();

            return builder;
        }
        private static void RegisterServerEvents(this ContainerBuilder builder)
        {
            var assembly = Assembly.GetAssembly(typeof(PlayerAddedOnMapEventHandler));
            builder.RegisterAssemblyTypes(assembly);
        }
        private static void RegisterGameEvents(this ContainerBuilder builder) => builder.RegisterAssembliesByInterface(typeof(IGameEventHandler));

        private static void RegisterEventSubscribers(this ContainerBuilder builder)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(types).As<ICreatureEventSubscriber>().SingleInstance();
            builder.RegisterAssemblyTypes(types).As<IItemEventSubscriber>().SingleInstance();
            builder.RegisterAssemblyTypes(types).As<IChatChannelEventSubscriber>().SingleInstance();
        }
    }
}
