using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NeoServer.Application.Features.Combat;
using NeoServer.Application.Features.Combat.Attacks;
using NeoServer.Application.Features.Combat.Attacks.AttackSelector;
using NeoServer.Application.Features.Combat.Attacks.MeleeAttack;
using NeoServer.Game.Combat;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Attacks.MeleeAttack;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class FeatureInjection
{
    public static IServiceCollection AddFeatures(this IServiceCollection builder)
    {
        AddCombatFeature(builder);
        return builder;
    }

    private static IServiceCollection AddCombatFeature(IServiceCollection builder)
    {
        var applicationAssembly = Assembly.GetAssembly(typeof(MeleeAttackStrategy));

        var gameAssembly = Assembly.GetAssembly(typeof(IAttackValidation));
        
        builder.RegisterAssembliesByInterface(typeof(IAttackValidation));
        builder.RegisterAssembliesByInterface(typeof(IAttackStrategy));
        
        builder.RegisterAssemblyTypes<IAttackStrategy>(gameAssembly, applicationAssembly);

        builder.AddSingleton<AttackCalculation>();

        builder.AddSingleton<PlayerAttackSelector>();
        builder.AddSingleton<AttackLibrary>();
        return builder;
    }
}