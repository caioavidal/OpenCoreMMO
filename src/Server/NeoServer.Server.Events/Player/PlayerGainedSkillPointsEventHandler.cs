using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class PlayerGainedSkillPointsEventHandler
    {
        private readonly IGameServer game;

        public PlayerGainedSkillPointsEventHandler(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer player, SkillType skill)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new PlayerSkillsPacket(player));
                connection.Send();
            }
        }
    }
}