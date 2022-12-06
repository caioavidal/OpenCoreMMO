using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Scripts.Lua.Functions;

public static class PlayerFunctions
{
    public static void AddPlayerFunctions(this NLua.Lua lua)
    {
        lua["addToPlayerBackpack"] = (IPlayer player, IItem item) =>
            player.Inventory.BackpackSlot is { } container && container.AddItem(item, true).Succeeded;
    }
}