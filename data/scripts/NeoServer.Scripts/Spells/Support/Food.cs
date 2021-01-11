using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Scripts.Spells.Support
{
    public class Food : Spell<Food>
    {
        public override EffectT Effect => EffectT.GlitterGreen;

        public override uint Duration => 0;

        public override ConditionType ConditionType => ConditionType.None;

        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            error = InvalidOperation.None;

            actor.CreateItem(2666, 100);
            return true;
        }
    }
}
