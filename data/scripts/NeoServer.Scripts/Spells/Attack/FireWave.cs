using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Game.Common.Creatures.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Scripts.Spells.Attack
{
    public class FireWave : Spell<FireWave>
    {
        public override EffectT Effect => EffectT.None;

        public override uint Duration => throw new NotImplementedException();

        public override ConditionType ConditionType => throw new NotImplementedException();

        public override void OnCast(ICombatActor actor)
        {
            throw new NotImplementedException();
        }
    }
}
