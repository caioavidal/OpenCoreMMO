using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Combat;

public readonly ref struct AttackInput(ICombatActor aggressor, IThing target)
{
    public ICombatActor Aggressor => aggressor;
    public IThing Target => target;
    public required AttackParameter Attack { get; init; }

}
public readonly struct AttackParameter
{
    public byte Range { get; init; }
    public ushort MinDamage { get; init; }
    public ushort MaxDamage { get; init; }
    public DamageType DamageType { get; init; }
    public EffectT Effect { get; init; }
    public byte Radius { get; init; }
    public bool NeedTarget { get; init; }
    public byte Length { get; init; }
    public byte Spread { get; init; }
    public ShootType ShootType { get; init; }
    public required string Name { get; init; }
    public ExtraAttack ExtraAttack { get; init; }
    public CooldownType CooldownType { get; init; }
    public bool HasExtraAttack => ExtraAttack.MaxDamage > 0;
}
public readonly struct ExtraAttack
{
    public ushort MinDamage { get; init; }
    public ushort MaxDamage { get; init; }
    public DamageType DamageType { get; init; } 
}