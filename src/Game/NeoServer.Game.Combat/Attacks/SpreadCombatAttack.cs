using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Effects.Magical;

namespace NeoServer.Game.Creatures.Combat.Attacks
{
    public class SpreadCombatAttack : CombatAttack
    {
        public SpreadCombatAttack(byte length, byte spread)
        {
            Length = length;
            Spread = spread;
        }

        public override bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option, out CombatAttackType combatType)
        {
            combatType = new CombatAttackType(option.DamageType);

            if (CalculateAttack(actor, enemy, option, out var damage))
            {
                combatType.Area = SpreadEffect.Create(actor.Location,actor.Direction, Length, Spread);
                actor.PropagateAttack(combatType.Area, damage);
                return true;
            }
            return false;
        }

        public byte Spread { get; }
        public byte Length { get; }
    }
}
