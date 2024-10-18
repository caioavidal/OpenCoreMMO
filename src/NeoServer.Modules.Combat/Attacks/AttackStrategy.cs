using NeoServer.Game.Combat;
using NeoServer.Game.Common.Results;

namespace NeoServer.Modules.Combat.Attacks;

public abstract class AttackStrategy : IAttackStrategy
{
    public abstract string Name { get; }

    public Result Execute(in AttackInput attackInput)
    {
        if (AttackParamsModifier.AttackParameterModifierFunction is not null)
            AttackParamsModifier.AttackParameterModifierFunction
            (
                attackInput.Aggressor,
                attackInput.Target,
                attackInput.Parameters
            );

        return Attack(attackInput);
    }

    protected abstract Result Attack(in AttackInput attackInput);
}