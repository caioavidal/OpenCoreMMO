namespace NeoServer.Game.Common.Contracts.Items.Weapons;

public interface IMagicalWeapon : IDistanceWeapon
{
    public ushort MaxHitChance { get; }
    public ushort MinHitChance { get; }
    public ushort ManaConsumption { get; }
}