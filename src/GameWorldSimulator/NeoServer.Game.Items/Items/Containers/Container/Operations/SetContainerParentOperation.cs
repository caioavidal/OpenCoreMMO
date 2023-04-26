using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.Containers.Container.Operations;

internal static class SetContainerParentOperation
{
    public static void SetParent(Container container, IThing thing)
    {
        container.Parent = thing;
        if (container.Parent is IPlayer) container.Location = new Location(Slot.Backpack);
        container.SetOwner(container.RootParent);
    }
}