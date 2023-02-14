namespace NeoServer.Game.Common.Contracts.Items;

public interface IMovableItem
{
    float Weight { get; }
    IThing Owner { get; }
    void SetOwner(IThing thing);
}