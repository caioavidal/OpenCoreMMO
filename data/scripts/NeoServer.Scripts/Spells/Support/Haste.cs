using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Spells;
using System;

namespace NeoServer.Scripts
{
    public class Haste : HasteSpell
    {
        public override EffectT Effect => EffectT.GlitterBlue;
        public override uint Duration => 20000;
        public override ushort SpeedBoost => 400;
    }
}
