using Autofac;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Creatures.Services;
using NeoServer.Server.Commands.Player.UseItem;

namespace NeoServer.Server.Standalone.IoC.Modules
{
    public static class ServiceInjection
    {
        public static ContainerBuilder AddServices(this ContainerBuilder builder)
        {
            //Game services
            builder.RegisterType<DealTransaction>().As<IDealTransaction>().SingleInstance();
            builder.RegisterType<CoinTransaction>().As<ICoinTransaction>().SingleInstance();
            builder.RegisterType<PartyShareExperienceService>().As<IPartyShareExperienceService>().SingleInstance();
            builder.RegisterType<PartyInviteService>().As<IPartyInviteService>().SingleInstance();
            builder.RegisterType<SummonService>().As<ISummonService>().SingleInstance();

            //application services
            builder.RegisterType<HotkeyService>().SingleInstance();

            return builder;
        }
    }
}