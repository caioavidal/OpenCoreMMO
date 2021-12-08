using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Movements;
using NeoServer.Server.Commands.Movements.ToContainer;
using NeoServer.Server.Commands.Movements.ToInventory;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Commands;

namespace NeoServer.Server.Commands.Player
{
    public class PlayerThrowItemCommand : ICommand
    {
        private readonly IGameServer game;
        private readonly IToMapMovementService toMapMovementService;

        public PlayerThrowItemCommand(IGameServer game, IToMapMovementService  toMapMovementService)
        {
            this.game = game;
            this.toMapMovementService = toMapMovementService;
        }

        public void Execute(IPlayer player, ItemThrowPacket itemThrow)
        {
            if (ContainerToContainerMovementOperation.IsApplicable(itemThrow))
                ContainerToContainerMovementOperation.Execute(player, itemThrow);
            else if (MapToInventoryMovementOperation.IsApplicable(itemThrow))
                MapToInventoryMovementOperation.Execute(player, game.Map, itemThrow);
            else if (ToMapMovementOperation.IsApplicable(itemThrow))
                ToMapMovementOperation.Execute(player, itemThrow, toMapMovementService);
            else if (InventoryToContainerMovementOperation.IsApplicable(itemThrow))
                InventoryToContainerMovementOperation.Execute(player, itemThrow);
            else if (ContainerToInventoryMovementOperation.IsApplicable(itemThrow))
                ContainerToInventoryMovementOperation.Execute(player, itemThrow);
            else if (MapToContainerMovementOperation.IsApplicable(itemThrow))
                MapToContainerMovementOperation.Execute(player, game, game.Map, itemThrow);
            else if (InventoryToInventoryOperation.IsApplicable(itemThrow))
                InventoryToInventoryOperation.Execute(player, itemThrow);
        }
    }
}