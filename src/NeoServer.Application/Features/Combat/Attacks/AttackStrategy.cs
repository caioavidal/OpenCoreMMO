using NeoServer.Game.Combat;
using NeoServer.Game.Common.Results;

namespace NeoServer.Application.Features.Combat.Attacks;

public abstract class AttackStrategy : IAttackStrategy
{
    protected abstract Result Attack(in AttackInput attackInput);
    public abstract string Name { get; }

    public Result Execute(in AttackInput attackInput)
    {
        if (AttackParamsModifier.AttackParameterModifierFunction is not null)
        {
            AttackParamsModifier.AttackParameterModifierFunction
            (
                attackInput.Aggressor,
                attackInput.Target,
                attackInput.Attack
            );
        }
        
        return Attack(attackInput);
    }
}