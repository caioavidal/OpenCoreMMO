using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Commands.Combat;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerAttackHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerAttackHandler(Game game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var creatureId = message.GetUInt32();
            if (game.CreatureManager.TryGetCreature(connection.PlayerId, out ICreature player))
            {
                game.Dispatcher.AddEvent(new Event(() => CreatureAttackCommand.Execute(player as IPlayer, creatureId, game, connection)));
            }
        }
    }
}
