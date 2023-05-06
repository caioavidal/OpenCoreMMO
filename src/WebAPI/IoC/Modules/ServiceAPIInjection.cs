using Autofac;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NeoServer.API.Helpers;

namespace NeoServer.Shared.IoC.Modules;

public static class ServiceAPIInjection
{
    public static IServiceCollection AddServicesAPI(this IServiceCollection services)
    {
        var scanAssemblies = AssemblyHelper.Instance().GetAllAssemblies();

        var servicesAndRepositories = scanAssemblies
            .SelectMany(o => o.DefinedTypes
                .Where(x => x.IsInterface)
                .Where(c => c.FullName.EndsWith("APIService"))
            );

        foreach (var typeInfo in servicesAndRepositories)
        {
            var types = scanAssemblies
                .SelectMany(o => o.DefinedTypes
                    .Where(x => x.IsClass)
                    .Where(x => typeInfo.IsAssignableFrom(x))
                );

            foreach (var type in types)
                services.TryAdd(new ServiceDescriptor(typeInfo, type, ServiceLifetime.Scoped));
        }

        return services;
    }
}
