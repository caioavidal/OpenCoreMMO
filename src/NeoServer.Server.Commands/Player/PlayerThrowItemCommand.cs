using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Movement;
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
                return;
            }
            if (MapToInventoryMovementOperation.IsApplicable(itemThrow))
            {
                //MapToContainerMovementOperation.Execute(player, game.Map, )
                MapToInventoryMovementOperation.Execute(player, game.Map, itemThrow);
            }
            if (ToMapMovementOperation.IsApplicable(itemThrow))
            {
                ToMapMovementOperation.Execute(player, game.Map, itemThrow);
            }
            if (ContainerToInventoryMovementOperation.IsApplicable(itemThrow))
            {
                ContainerToInventoryMovementOperation.Execute(player, itemThrow);
            }
            if (MapToContainerMovementOperation.IsApplicable(itemThrow))
            {
                MapToContainerMovementOperation.Execute(player, game.Map, itemThrow);
            }
        }

    }
}
