using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Extensions.Spells.Support;

public class Haste : HasteSpell
{
    public override EffectT Effect => EffectT.GlitterBlue;
    public override uint Duration => 20000;
    public override ushort SpeedBoost => 400;
}