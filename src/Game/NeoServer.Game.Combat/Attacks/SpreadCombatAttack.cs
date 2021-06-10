using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Effects.Magical;

namespace NeoServer.Game.Combat.Attacks
{
    public class SpreadCombatAttack : CombatAttack
    {
        public SpreadCombatAttack(byte length, byte spread)
        {
            Length = length;
            Spread = spread;
        }

        public byte Spread { get; }
        public byte Length { get; }

        public override bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option,
            out CombatAttackType combatType)
        {
            combatType = new CombatAttackType(option.DamageType);

            if (CalculateAttack(actor, enemy, option, out var damage))
            {
                combatType.Area = SpreadEffect.Create(actor.Location, actor.Direction, Length, Spread);
                actor.PropagateAttack(combatType.Area, damage);
                return true;
            }

            return false;
        }
    }
}