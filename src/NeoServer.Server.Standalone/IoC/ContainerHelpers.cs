using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Standalone.IoC
{
    public static class ContainerHelpers
    {
        public static void RegisterAssembliesByInterface(this ContainerBuilder builder, Type interfaceType)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => interfaceType.IsAssignableFrom(x));

            foreach (var type in types)
            {
                if (type == interfaceType) continue;
                builder.RegisterType(type).SingleInstance();

            }
        }
    }
}
