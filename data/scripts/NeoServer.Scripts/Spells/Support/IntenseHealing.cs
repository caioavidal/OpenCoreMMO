using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Game.Enums.Creatures.Players;
using System;

namespace NeoServer.Scripts
{
    public class IntenseHealing : Spell<IntenseHealing>
    {
        public override EffectT Effect => EffectT.GlitterBlue;
        public override uint Duration => 0;

        public override ConditionType ConditionType => ConditionType.None;
        public override void OnCast(ICombatActor actor) => actor.Heal(100);
    }
}