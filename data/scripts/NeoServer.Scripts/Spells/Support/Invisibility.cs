using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Game.Common.Creatures.Players;
using System;

namespace NeoServer.Scripts
{
    public class Invisibility : Spell<Invisibility>
    {

        public override EffectT Effect => EffectT.GlitterBlue;
        public override uint Duration => 20000;
        public override ConditionType ConditionType => ConditionType.Haste;

        public override void OnCast(ICombatActor actor) => actor.TurnInvisible();
        
        public override void OnEnd(ICombatActor actor)
        {
            actor.TurnVisible();
            base.OnEnd(actor);
        }

    }
}
