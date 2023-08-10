using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack.Mage;

public class TerraWave : WaveSpell
{
    public override DamageType DamageType => DamageType.Earth;
    public override string AreaName => "AREA_SQUAREWAVE5";
}