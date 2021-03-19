using NeoServer.Game.Common.Creatures;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerSkillsPacket : OutgoingPacket
    {
        private readonly IPlayer player;
        public PlayerSkillsPacket(IPlayer player)
        {
            this.player = player;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.PlayerSkills);
            message.AddByte(player.GetSkillInfo(SkillType.Fist));
            message.AddByte(player.GetSkillPercent(SkillType.Fist));

            message.AddByte(player.GetSkillInfo(SkillType.Club));
            message.AddByte(player.GetSkillPercent(SkillType.Club));

            message.AddByte(player.GetSkillInfo(SkillType.Sword));
            message.AddByte(player.GetSkillPercent(SkillType.Sword));

            message.AddByte(player.GetSkillInfo(SkillType.Axe));
            message.AddByte(player.GetSkillPercent(SkillType.Axe));

            message.AddByte(player.GetSkillInfo(SkillType.Distance));
            message.AddByte(player.GetSkillPercent(SkillType.Distance));

            message.AddByte(player.GetSkillInfo(SkillType.Shielding));
            message.AddByte(player.GetSkillPercent(SkillType.Shielding));

            message.AddByte(player.GetSkillInfo(SkillType.Fishing));
            message.AddByte(player.GetSkillPercent(SkillType.Fishing));
        }
    }
}
