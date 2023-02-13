namespace NeoServer.Game.Common.Contracts.Items.Types.Body;

public interface IThrowableDistanceWeaponItem : ICumulative, IWeapon
{
    byte AttackPower { get; }
    byte Range { get; }
}