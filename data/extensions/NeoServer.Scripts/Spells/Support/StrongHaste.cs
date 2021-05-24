using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Combat.Spells;

namespace NeoServer.Scripts
{
    public class StrongHaste : HasteSpell
    {
        public override EffectT Effect => EffectT.GlitterBlue;
        public override uint Duration => 20000;
        public override ushort SpeedBoost => 600;

    }
}
