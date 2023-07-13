using System;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Common.Combat.Structs;

public ref struct CombatAttackCalculationValue
{
    public CombatAttackCalculationValue(ushort minDamage, ushort maxDamage, DamageType damageType) : this()
    {
        MinDamage = minDamage;
        MaxDamage = maxDamage;
        DamageType = damageType;
    }

    public CombatAttackCalculationValue(ushort minDamage, ushort maxDamage, byte range, DamageType damageType)
    {
        Range = range;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
        DamageType = damageType;
        DamageEffect = EffectT.None;
    }

    public byte Range { get; set; }
    public ushort MinDamage { get; set; }
    public ushort MaxDamage { get; set; }
    public DamageType DamageType { get; set; }
    public EffectT DamageEffect { get; set; }
}

public class CombatAttackParams
{
    public CombatAttackParams()
    {
        
    }
    public CombatAttackParams(ShootType shootType) : this()
    {
        ShootType = shootType;
    }

    public CombatAttackParams(DamageType damageType) : this()
    {
        DamageType = damageType;
    }
    public bool Missed { get; set; }
    public ShootType ShootType { get; set; }
    public DamageType DamageType { get; set; }
    public EffectT EffectT { get; set; }
    public AffectedLocation[] Area { get; set; }
    public CombatDamage[] Damages { get; set; }

    public void SetDamageType(int damageType) => DamageType = (DamageType)damageType;
    public void SetEffect(int effect) => EffectT = (EffectT)effect;

    public void SetArea(Coordinate[] coordinates)
    {
        if (coordinates is null)
        {
            Area = Array.Empty<AffectedLocation>();
            return;
        }

        Area = new AffectedLocation[coordinates.Length];

        var i = 0;
        foreach (var coordinate in coordinates) Area[i++] = new AffectedLocation(coordinate);
    }
}

public class AffectedLocation
{
    public AffectedLocation(Coordinate coordinate)
    {
        Point = coordinate;
        Missed = false;
    }

    public Coordinate Point { get; }
    public bool Missed { get; private set; }

    public void MarkAsMissed()
    {
        Missed = true;
    }
}