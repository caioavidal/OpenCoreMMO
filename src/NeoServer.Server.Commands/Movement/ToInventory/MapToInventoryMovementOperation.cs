
using NeoServer.Game.Common.Location;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Commands.Movement
{
    public sealed class MapToInventoryMovementOperation
    {
        public static void Execute(IPlayer player, Game game, IMap map, ItemThrowPacket itemThrow) => WalkToMechanism.DoOperation(player, () => FromMapToInventory(player, map, itemThrow), itemThrow.FromLocation, game);

        private static void FromMapToInventory(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {
            if (map[itemThrow.FromLocation] is not ITile fromTile) return;
            if (fromTile.TopItemOnStack is not IItem item) return;

            player.MoveItem(fromTile, player.Inventory, item, itemThrow.Count, 0, (byte)itemThrow.ToLocation.Slot);
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
             itemThrowPacket.FromLocation.Type == LocationType.Ground
             && itemThrowPacket.ToLocation.Type == LocationType.Slot;
    }
}
