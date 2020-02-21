using Autofac;
using NeoServer.Data.RavenDB;
using NeoServer.Server.Contracts.Repositories;

namespace NeoServer.Server.Standalone.IoC
{
    public class IoC
    {
        public static IContainer Container { get; private set; }

        public static void Register()
        {
            if (Container == null)
            {
                var builder = new ContainerBuilder();
                builder.RegisterType<AccountRepository>().As<IAccountRepository>();
                Container = builder.Build();
            }
        }
    }
}
