using NeoServer.API.Filters;
using NeoServer.API.Helpers;

namespace NeoServer.Shared.IoC.Modules;

public static class ApiBehavioursInjection
{
    public static IServiceCollection AddBehaviours(this IServiceCollection services)
    {
        services.AddMvcCore().ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context => ValidationHelper.GetInvalidModelStateResponse(context);
        });

        services.AddMvcCore(options => options.Filters.Add<ExceptionFilter>());

        return services;
    }
}