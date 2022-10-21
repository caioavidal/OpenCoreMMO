using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Combat.Attacks;

public class AttackValidation
{
    public static Result CanAttack(ICreature attacker, ICreature victim)
    {
        if (attacker.Tile.ProtectionZone)
        {
            return Result.Fail(InvalidOperation.CannotAttackWhileInProtectionZone);
        }

        if (victim.Tile.ProtectionZone)
        {
            return Result.Fail(InvalidOperation.CannotAttackPersonInProtectionZone);
        }

        return Result.Success;
    }
}