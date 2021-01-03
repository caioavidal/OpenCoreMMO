using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Spells
{
    public abstract class AttackSpell: Spell<AttackSpell>
    {
        public override EffectT Effect => EffectT.GlitterBlue;
        public override uint Duration => 0;
        public override ConditionType ConditionType => ConditionType.None;

        public override void OnCast(ICombatActor actor)
        {
            //actor.Attack()
        }
    }
}
