using NeoServer.Game.Common.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Server.Events
{
    public class PlayerGainedSkillPointsEventHandler
    {
        private readonly IGameServer game;
        public PlayerGainedSkillPointsEventHandler(IGameServer game) => this.game = game;
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
