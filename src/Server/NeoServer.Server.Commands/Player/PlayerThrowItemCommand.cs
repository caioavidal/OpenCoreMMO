using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Movement;
using NeoServer.Server.Commands.Movement.ToInventory;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerThrowItemCommand : Command
    {
        private readonly Game game;
        private ItemThrowPacket itemThrow;
        private readonly IPlayer player;

        public PlayerThrowItemCommand(IPlayer player, ItemThrowPacket itemThrow, Game game)
        {
            this.itemThrow = itemThrow;
            this.game = game;
            this.player = player;
        }

        public override void Execute()
        {
        
            if (ContainerToContainerMovementOperation.IsApplicable(itemThrow))
            {
                ContainerToContainerMovementOperation.Execute(player, itemThrow);
            }
            else if (MapToInventoryMovementOperation.IsApplicable(itemThrow))
            {
                MapToInventoryMovementOperation.Execute(player,game, game.Map, itemThrow);
            }
            else if (ToMapMovementOperation.IsApplicable(itemThrow))
            {
                ToMapMovementOperation.Execute(player, game, game.Map, itemThrow);
            }
            else if (InventoryToContainerMovementOperation.IsApplicable(itemThrow))
            {
                InventoryToContainerMovementOperation.Execute(player, itemThrow);
            }
            else if (ContainerToInventoryMovementOperation.IsApplicable(itemThrow))
            {
                ContainerToInventoryMovementOperation.Execute(player, itemThrow);
            }
            else if (MapToContainerMovementOperation.IsApplicable(itemThrow))
            {
                MapToContainerMovementOperation.Execute(player, game, game.Map, itemThrow);
            }
            else if (InventoryToInventoryOperation.IsApplicable(itemThrow))
            {
                InventoryToInventoryOperation.Execute(player, itemThrow);
            }
        }
    }
}
