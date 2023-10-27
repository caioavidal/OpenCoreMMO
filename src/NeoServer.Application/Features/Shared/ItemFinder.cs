using NeoServer.Application.Features.UseItem;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Application.Features.Shared;

public class ItemFinder
{
    private readonly HotkeyService _hotkeyService;
    private readonly IMap _map;

    public ItemFinder(HotkeyService hotkeyService, IMap map)
    {
        _hotkeyService = hotkeyService;
        _map = map;
    }

    public IItem Find(IPlayer player, Location location, ushort itemClientId)
    {
        if (location.IsHotkey)
            return _hotkeyService.GetItem(player, itemClientId);

        if (location.Type is LocationType.Ground && _map[location] is { } tile)
            return tile.TopItemOnStack;

        if (location.Slot == Slot.Backpack)
            return player.Inventory[Slot.Backpack];

        if (location.Type is LocationType.Slot &&
            player.Inventory[location.Slot] is { } item)
            return item;

        if (location.Type is LocationType.Container &&
            player.Containers[location.ContainerId][location.ContainerSlot] is { } itemInContainer)
            return itemInContainer;

        return null;
    }
}