using NeoServer.Application.Features.Combat.Attacks.DistanceAttack;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Weapons;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Combat.Attacks.DistanceAttack;

public readonly ref struct MissAttackCalculationValues(Location origin, Location target, IWeapon weapon, ushort skill)
{
    public Location Origin => origin;
    public Location Target => target;
    public ushort Skill => skill;
    public IWeapon Weapon => weapon;
}

public readonly ref struct MissAttackResult(Location destination = default)
{
    public static MissAttackResult NotMissed => new MissAttackResult(Location.Zero) { Missed = false };
    public bool Missed { get; private init; }= destination != default;
    public Location Destination => destination;
}

public class MissAttackCalculation
{
    public static MissAttackResult Calculate(MissAttackCalculationValues missAttackCalculationValues)
    {
        var distance = (byte)missAttackCalculationValues.Origin.GetSqmDistance(missAttackCalculationValues.Target);
        var weapon = missAttackCalculationValues.Weapon;
        var skill = missAttackCalculationValues.Skill;
        var origin = missAttackCalculationValues.Origin;
        var target = missAttackCalculationValues.Target;
        
        var hitChance = GetHitChance(weapon, skill, distance);

        var attackMissed = ValidateIfAttackMissed(hitChance);

        if (!attackMissed) return new();

        var destination = GetRandomAttackDestination(origin, target);
        return new(destination);
    }

    private static byte GetHitChance(IWeapon weapon, ushort skill, byte distance)
    {
        byte hitChance = 100;

        if (weapon is IDistanceWeapon distanceWeapon)
        {
            hitChance =
                (byte)(DistanceHitChanceCalculation.CalculateFor2Hands(skill, distance) +
                       distanceWeapon.ExtraHitChance);
        }

        if (weapon is IThrowableWeapon throwableDistanceWeapon)
        {
            hitChance =
                (byte)(DistanceHitChanceCalculation.CalculateFor1Hand(skill, distance) +
                       throwableDistanceWeapon.ExtraHitChance);
        }

        return hitChance;
    }

    private static bool ValidateIfAttackMissed(byte hitChance)
    {
        var value = GameRandom.Random.Next(1, maxValue: 100);
        return hitChance < value;
    }

    private static Location GetRandomAttackDestination(Location originLocation, Location targetLocation)
    {
        Location destLocation;
        do
        {
            var index = GameRandom.Random.Next(0, maxValue: targetLocation.Neighbours.Length);
            destLocation = targetLocation.Neighbours[index];
        } while (destLocation == originLocation);

        return destLocation;
    }
}