using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack;

public class FireWave : WaveSpell
{
    public override string AreaName => "AREA_WAVE4";
    public override DamageType DamageType => DamageType.Fire;
    public override byte Range => 1;
  
}