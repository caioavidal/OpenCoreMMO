namespace NeoServer.Game.Common.Contracts.Items.Types;

public interface IPickupable : IMovableThing, IItem
{
    float Weight { get; }
}