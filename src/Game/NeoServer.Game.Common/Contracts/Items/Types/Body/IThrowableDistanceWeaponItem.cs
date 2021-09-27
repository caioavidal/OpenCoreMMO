namespace NeoServer.Game.Common.Contracts.Items.Types.Body
{
    public interface IThrowableDistanceWeaponItem : ICumulative, IWeapon
    {
        byte Attack { get; }
        byte Range { get; }
    }
}