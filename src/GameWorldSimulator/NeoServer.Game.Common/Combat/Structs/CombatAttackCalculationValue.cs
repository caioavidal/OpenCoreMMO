using System;
using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
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
    public AffectedArea Area { get; private set; }
    public CombatDamage[] Damages { get; set; }
    public bool IsAreaAttack => (Area?.Any() ?? false) || !string.IsNullOrWhiteSpace(AreaName);
    public void SetDamageType(int damageType) => DamageType = (DamageType)damageType;
    public void SetEffect(int effect) => EffectT = (EffectT)effect;
    public string AreaName { get; set; }

    public void SetArea(AffectedLocation[] affectedLocations) => Area = new AffectedArea(affectedLocations);
    public void SetArea(Coordinate[] coordinates) => Area = new AffectedArea(coordinates);
}

public class AffectedArea
{
    public AffectedArea(Coordinate[] coordinates)
    {
        if (coordinates is null)
        {
            AffectedLocations = Array.Empty<AffectedLocation>();
            return;
        }

        AffectedLocations = new AffectedLocation[coordinates.Length];

        var i = 0;
        foreach (var coordinate in coordinates) AffectedLocations[i++] = new AffectedLocation(coordinate);
    }

    public AffectedArea() => AffectedLocations = Array.Empty<AffectedLocation>();

    public AffectedArea(AffectedLocation[] affectedLocations) =>
        AffectedLocations = affectedLocations ?? Array.Empty<AffectedLocation>();

    public AffectedLocation[] AffectedLocations { get; }
    public ICombatActor[] AffectedCreatures { get; private set; }
    public bool HasAnyLocationAffected { get; private set; }
    public bool Any() => AffectedLocations?.Any() ?? false;
    public bool IsProcessed { get; private set; }

    public void MarkAsProcessed(bool hasAnyLocationAffected, ICombatActor[] affectedCreatures)
    {
        HasAnyLocationAffected = hasAnyLocationAffected;
        AffectedCreatures = affectedCreatures;
        IsProcessed = true;
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