using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Commands.Player.UseItem;

public class ItemFinderService
{
    private readonly HotkeyService _hotkeyService;
    private readonly IGameServer _gameServer;

    public ItemFinderService(HotkeyService hotkeyService, IGameServer gameServer)
    {
        _hotkeyService = hotkeyService;
        _gameServer = gameServer;
    }
    
    public IItem Find(IPlayer player, Location itemLocation, ushort clientId)
    {
        IItem item = null;
        if (itemLocation.IsHotkey)
        {
            item = _hotkeyService.GetItem(player, clientId);
        }
        else if (itemLocation.Type == LocationType.Ground)
        {
            if (_gameServer.Map[itemLocation] is not { } tile) return null;
            item = tile.TopItemOnStack;
        }
        else if (itemLocation.Slot == Slot.Backpack)
        {
            item = player.Inventory[Slot.Backpack];
            item.SetNewLocation(itemLocation);
        }
        else if (itemLocation.Type == LocationType.Container)
        {
            item = player.Containers[itemLocation.ContainerId][itemLocation.ContainerSlot];
            item.SetNewLocation(itemLocation);
        }

        return item;
    }
}