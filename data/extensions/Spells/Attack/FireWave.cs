using NeoServer.Game.Common;
using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack
{
    public class FireWave : WaveSpell
    {
        protected override string AreaName => "AREA_WAVE4";
        public override DamageType DamageType => DamageType.Fire;
        public override MinMax Damage => new(5, 100);
        public override byte Range => 1;
    }
}