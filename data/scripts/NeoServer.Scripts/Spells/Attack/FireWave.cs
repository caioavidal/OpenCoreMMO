using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Contracts.Creatures;
using System;

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
