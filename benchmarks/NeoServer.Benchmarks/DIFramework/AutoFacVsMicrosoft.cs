using Autofac;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.Extensions.DependencyInjection;

namespace NeoServer.Benchmarks.DIFramework;

[SimpleJob(RunStrategy.ColdStart, 5)]
public class AutoFacVsMicrosoft
{
    private IContainer _autoFacContainer;
    private ServiceProvider _serviceProviderContainer;

    public AutoFacVsMicrosoft()
    {
        SetupAutoFacContainer();
        SetupServiceProviderContainer();
    }

    private void SetupAutoFacContainer()
    {
        var containerBuilder = new ContainerBuilder();

        containerBuilder.RegisterType<SingletonService>().SingleInstance();
        containerBuilder.RegisterType<Singleton2Service>().SingleInstance();

        _autoFacContainer = containerBuilder.Build();
    }

    private void SetupServiceProviderContainer()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<SingletonService>();
        serviceCollection.AddSingleton<Singleton2Service>();
        _serviceProviderContainer = serviceCollection.BuildServiceProvider();
    }

    [Benchmark]
    public void GetInstanceUsingAutofac()
    {
        for (var i = 0; i < 1_000_000; i++) _ = _autoFacContainer.Resolve<SingletonService>();
    }

    [Benchmark]
    public void GetInstanceUsingServiceProvider()
    {
        for (var i = 0; i < 1_000_000; i++) _ = _serviceProviderContainer.GetService<SingletonService>();
    }
}

public class SingletonService
{
    public SingletonService(Singleton2Service singleton2Service)
    {
    }
}

public class Singleton2Service
{
}