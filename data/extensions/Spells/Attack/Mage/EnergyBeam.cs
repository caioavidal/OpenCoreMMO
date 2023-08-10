﻿using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack.Mage;

public class EnergyBeam : WaveSpell
{
    public override DamageType DamageType => DamageType.Energy;
    public override string AreaName => "AREA_BEAM5";
}