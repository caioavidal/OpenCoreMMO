using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Game.Common.Creatures.Players;
using System;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;

namespace NeoServer.Scripts
{
    public class Invisibility : Spell<Invisibility>
    {

        public override EffectT Effect => EffectT.GlitterBlue;
        public override uint Duration => 20000;
        public override ConditionType ConditionType => ConditionType.Haste;

        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            error = InvalidOperation.None;
            actor.TurnInvisible();
            return true;
        }
        
        public override void OnEnd(ICombatActor actor)
        {
            actor.TurnVisible();
            base.OnEnd(actor);
        }

    }
}
