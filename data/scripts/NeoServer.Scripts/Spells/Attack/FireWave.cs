using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using System;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;

namespace NeoServer.Scripts.Spells.Attack
{
    public class FireWave : Spell<FireWave>
    {
        public override EffectT Effect => EffectT.None;

        public override uint Duration => throw new NotImplementedException();

        public override ConditionType ConditionType => throw new NotImplementedException();

        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            throw new NotImplementedException();
        }
    }
}
