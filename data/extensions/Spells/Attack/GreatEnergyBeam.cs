using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack;

public class GreatEnergyBeam : WaveSpell
{
    public override DamageType DamageType => DamageType.Energy;
    public override EffectT AreaEffect => EffectT.BubbleBlue;
    public override string AreaName => "AREA_BEAM7";
    
    public override MinMax GetFormula(ICombatActor actor)
    {
        if (actor is not IPlayer player) return new MinMax(0, 0);

        var magicLevel = player.Skills[SkillType.Magic].Level;

        var min = player.MinimumAttackPower + (magicLevel * 3.6) + 22;
        var max = player.MinimumAttackPower + (magicLevel * 6) + 37;

        return new MinMax((int)min, (int)max);
    }
}