using Autofac;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Creatures.Events;
using NeoServer.Game.Creatures.Services;
using NeoServer.Server.Commands.Player.UseItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Standalone.IoC
{
    public static class ServiceInjection
    {
        public static ContainerBuilder AddServices(this ContainerBuilder builder)
        {
            //Game services
            builder.RegisterType<DealTransaction>().As<IDealTransaction>().SingleInstance();
            builder.RegisterType<CoinTransaction>().As<ICoinTransaction>().SingleInstance();
            builder.RegisterType<PartyInviteService>().As<IPartyInviteService>().SingleInstance();
            builder.RegisterType<SummonService>().As<ISummonService>().SingleInstance();

            //application services
            builder.RegisterType<HotkeyService>().SingleInstance();

            return builder;
        }
    }
}
