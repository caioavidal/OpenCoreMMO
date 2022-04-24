using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Game.Common.Contracts.Items;

public interface IMovableItem
{
    void SetOwner(IThing thing);
    IThing Owner { get; }
}