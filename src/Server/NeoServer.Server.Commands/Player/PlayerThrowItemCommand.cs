using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Movement;
using NeoServer.Server.Commands.Movement.ToInventory;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Commands;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerThrowItemCommand : ICommand
    {
        private readonly IGameServer game;

        public PlayerThrowItemCommand(IGameServer game)
        {
            
            this.game = game;
            
        }

        public void Execute(IPlayer player, ItemThrowPacket itemThrow)
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
