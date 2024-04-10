using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Combat;

public readonly ref struct PreAttackValues
{
    public required ICombatActor Aggressor { get; init; }
    public required IThing Target { get; init; }
    public Location MissLocation { get; init; }
    public bool Missed => MissLocation != Location.Zero;
    public Location AttackDestination => Missed ? MissLocation : Target.Location;
    public ShootType ShootType { get; init; }
}