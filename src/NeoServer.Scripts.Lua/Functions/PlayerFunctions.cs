using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Scripts.Lua.Functions;

public static class PlayerFunctions
{
    public static void AddPlayerFunctions(this NLua.Lua lua)
    {
        lua.DoString("player_helper = {}");
        lua["player_helper.addToBackpack"] = AddToBackpackFunction;
    }

    private static LuaResult AddToBackpackFunction(IPlayer player, params IItem[] items)
    {
        var result = AddToBackpack(player, items);
        return new LuaResult(result.Succeeded, result.Error);
    }

    private static Result<IPickupable> AddToBackpack(IPlayer player, params IItem[] items)
    {
        if (Guard.AnyNull(player, items)) return Result<IPickupable>.NotPossible;

        if (!items.Any()) return Result<IPickupable>.Fail(InvalidOperation.NotPossible);
        
        if(player.Inventory.BackpackSlot is not { } backpack) return Result<IPickupable>.Fail(InvalidOperation.NotEnoughRoom);

        float totalWeight = 0;
        
        foreach (var item in items)
        {
            if (item is not IPickupable pickupable) continue;
            totalWeight += pickupable.Weight;
        }

        if (player.TotalCapacity < totalWeight) return Result<IPickupable>.Fail(InvalidOperation.TooHeavy);

        if (backpack.TotalOfFreeSlots < items.Length)
            return Result<IPickupable>.Fail(InvalidOperation.NotEnoughRoom);

        foreach (var item in items)
        {
            player.Inventory.TryAddItemToSlot(Slot.Backpack, (IPickupable)item);
        }

        return Result<IPickupable>.Success;
    }
}