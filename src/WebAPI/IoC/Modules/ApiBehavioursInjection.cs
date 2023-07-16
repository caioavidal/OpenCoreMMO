using NeoServer.Web.API.Helpers;
using NeoServer.Web.API.HttpFilters;

namespace NeoServer.Web.API.IoC.Modules;

public static class ApiBehavioursInjection
{
    public static IServiceCollection AddBehaviours(this IServiceCollection services)
    {
        services.AddMvcCore().ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory =
                context => ValidationHelper.GetInvalidModelStateResponse(context);
        });

        services.AddMvcCore(options => options.Filters.Add<ExceptionFilter>());

        return services;
    }
}