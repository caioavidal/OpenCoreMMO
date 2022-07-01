using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack
{
    public class GreatEnergyBeam : WaveSpell
    {
        public override DamageType DamageType => DamageType.Energy;
        public override EffectT DamageEffect => EffectT.BubbleBlue;
        public override MinMax CalculateDamage(ICombatActor actor) => new(5, 100);
        protected override string AreaName => "AREA_BEAM7";
    }
}