using NeoServer.Game.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Application.Features.Combat;

//this class is used for script injection
public static class AttackParamsModifier
{
    public static Action<ICombatActor, IThing, AttackParameter> AttackParameterModifierFunction { get; set; }

    public static void Modify(ICombatActor aggressor, IThing victim, AttackParameter attackParameter)
    {
        AttackParameterModifierFunction?.Invoke(aggressor, victim, attackParameter);
    }
}