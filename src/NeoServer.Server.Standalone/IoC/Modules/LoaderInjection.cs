using Autofac;
using NeoServer.Loaders.Interfaces;
using NeoServer.Loaders.Items;
using NeoServer.Loaders.Monsters;
using NeoServer.Loaders.Spawns;
using NeoServer.Loaders.Spells;
using NeoServer.Loaders.Vocations;
using NeoServer.Server.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Standalone.IoC
{
    public static class LoaderInjection
    {
        public static ContainerBuilder AddLoaders(this ContainerBuilder builder)
        {
            builder.RegisterType<ItemTypeLoader>().SingleInstance();
            builder.RegisterType<Loaders.World.WorldLoader>().SingleInstance();
            builder.RegisterType<SpawnLoader>().SingleInstance();
            builder.RegisterType<MonsterLoader>().SingleInstance();
            builder.RegisterType<VocationLoader>().SingleInstance();
            builder.RegisterPlayerLoaders();
            builder.RegisterStartupLoaders();
            builder.RegisterType<SpellLoader>().SingleInstance();
            builder.RegisterCustomLoaders();

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).As<IRunBeforeLoaders>().SingleInstance();
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).As<IStartup>().SingleInstance();


            return builder;
        }

        private static void RegisterPlayerLoaders(this ContainerBuilder builder)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(types).As<IPlayerLoader>().SingleInstance();
        }
        private static void RegisterStartupLoaders(this ContainerBuilder builder)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(types).As<IStartupLoader>().SingleInstance();
        }
        private static void RegisterCustomLoaders(this ContainerBuilder builder) => builder.RegisterAssembliesByInterface(typeof(ICustomLoader));

    
    }
}
