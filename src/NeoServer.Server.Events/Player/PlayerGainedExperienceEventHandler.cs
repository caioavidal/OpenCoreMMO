using BenchmarkDotNet.Disassemblers;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerGainedExperienceEventHandler
    {
        private readonly Game game;

        public PlayerGainedExperienceEventHandler(Game game)
        {

            this.game = game;
        }
        public void Execute(ICreature player, uint experience)
        {

            foreach (var playerId in game.Map.GetPlayersAtPositionZone(player.Location))
            {
                if (game.CreatureManager.GetPlayerConnection(playerId, out var connection))
                {
                    connection.OutgoingPackets.Enqueue(new AnimatedTextPacket(player.Location, TextColor.White, experience.ToString()));

                    if (playerId == player.CreatureId)
                    {
                        connection.OutgoingPackets.Enqueue(new PlayerStatusPacket((IPlayer)player));
                    }

                    connection.Send();
                }
            }

        }
    }
}
