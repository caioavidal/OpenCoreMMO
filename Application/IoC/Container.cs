using Autofac;

public class IoC
{
    public static IContainer Container { get; private set; }

    public static void Load()
    {
        if (Container == null)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LoginHandler>().As<ICommandHandler<LoginCommand>>();
            Container = builder.Build();
        }
    }

    public static ICommandHandler<T> GetCommandHandler<T>() where T : ICommand =>
    Container.Resolve<ICommandHandler<T>>();

}