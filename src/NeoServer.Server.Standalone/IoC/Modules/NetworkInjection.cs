using Autofac;
using NeoServer.Networking.Listeners;
using NeoServer.Networking.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Standalone.IoC
{
    public static class NetworkInjection
    {
        public static ContainerBuilder AddNetwork(this ContainerBuilder builder)
        {
            builder.RegisterType<LoginProtocol>().SingleInstance();
            builder.RegisterType<GameProtocol>().SingleInstance();
            builder.RegisterType<LoginListener>().SingleInstance();
            builder.RegisterType<GameListener>().SingleInstance();
            return builder;
        }
    
    }
}
