using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack.Mage;

public class IceWave : WaveSpell
{
    public override string AreaName => "AREA_WAVE4";
    public override DamageType DamageType => DamageType.Ice;
    public override byte Range => 1;
}