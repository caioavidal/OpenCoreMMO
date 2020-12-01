using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Game.Common.Creatures.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Scripts.Spells.Support
{
    public class Food : Spell<Food>
    {
        public override EffectT Effect => EffectT.GlitterGreen;

        public override uint Duration => 0;

        public override ConditionType ConditionType => ConditionType.None;

        public override void OnCast(ICombatActor actor)
        {
            actor.CreateItem(2666, 100);
        }
    }
}
