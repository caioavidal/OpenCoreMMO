using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Combat.Attacks;

namespace NeoServer.Game.Combat.Attacks
{
    public class DrainCombatAttack : DistanceAreaCombatAttack
    {
        public DrainCombatAttack(byte range, byte radius, ShootType shootType) : base(range, radius, shootType)
        {
        }

        public override bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option, out CombatAttackType combatType)
        {
            combatType = new CombatAttackType(ShootType);

            if (CalculateAttack(actor, enemy, option, out var damage))
            {
                combatType.DamageType = option.DamageType;

                enemy.ReceiveAttack(actor, damage);
                return true;
            }

            return false;
        }
    }
}
