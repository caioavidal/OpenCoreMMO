using System;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Extensions.Spells.Attack
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