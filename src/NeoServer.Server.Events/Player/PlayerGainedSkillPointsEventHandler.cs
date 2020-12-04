using BenchmarkDotNet.Disassemblers;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Server.Events
{
    public class PlayerGainedSkillPointsEventHandler
    {
        private readonly Game game;
        public PlayerGainedSkillPointsEventHandler(Game game) => this.game = game;
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
