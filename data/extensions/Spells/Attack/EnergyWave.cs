using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack
{
    public class EnergyWave : WaveSpell
    {
        public override DamageType DamageType => DamageType.Energy;
        public override EffectT DamageEffect => EffectT.BubbleBlue;
        public override MinMax Damage => new(0, 5);
        protected override string AreaName => "AREA_SQUAREWAVE5";
    }
}