using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Spells;

namespace NeoServer.Game.Creatures.Combat.Attacks
{
    public class SpeedCombatAttack : DistanceCombatAttack
    {
        public SpeedCombatAttack(uint duration, short speedChance, byte range, ShootType shootType) : base(range, shootType)
        {
            SpeedChange = speedChance;

            if (SpeedChange < -1000)
                SpeedChange = -10000;

            Duration = duration;
        }

        public override bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option, out CombatAttackType combatType)
        {
            combatType = new CombatAttackType(option.DamageType);

            if (CalculateAttack(actor, enemy, option, out var damage))
            {
                return SpeedChange > 0 ? HasteSpell.Instance.InvokeOn(actor, enemy,null, out var error) : ParalyzeSpell.Instance.InvokeOn(actor, enemy,null, out error);
            }
            return false;
        }
        public uint Duration { get; } = 10000;

        public short SpeedChange { get; set; }
    }
}
