namespace NeoServer.Game.Common.Contracts.Items.Types.Body;

public interface IThrowableWeapon : ICumulative, IWeaponItem
{
    byte Range { get; }
    byte ExtraHitChance { get; }
    bool ShouldBreak();
}