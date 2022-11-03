namespace NeoServer.Game.Common.Contracts.Items;

public interface IMovableItem
{
    IThing Owner { get; }
    void SetOwner(IThing thing);
}