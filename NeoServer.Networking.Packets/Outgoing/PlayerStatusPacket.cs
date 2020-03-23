using NeoServer.Game.Creatures.Enums;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerStatusPacket:OutgoingPacket
    {
        public PlayerStatusPacket(IPlayer player) : base(false)
        {
            OutputMessage.AddByte((byte)GameOutgoingPacketType.PlayerStatus);
            OutputMessage.AddUInt16((ushort)Math.Min(ushort.MaxValue, player.Hitpoints));
            OutputMessage.AddUInt16((ushort)Math.Min(ushort.MaxValue, player.MaxHitpoints));
            OutputMessage.AddUInt32(Convert.ToUInt32(player.CarryStrength));
            
            OutputMessage.AddUInt32(Math.Min(0x7FFFFFFF, player.Experience)); // Experience: Client debugs after 2,147,483,647 exp
            
            OutputMessage.AddUInt16(player.Level);
            OutputMessage.AddByte(player.LevelPercent);
            OutputMessage.AddUInt16((ushort)Math.Min(ushort.MaxValue, player.Manapoints));
            OutputMessage.AddUInt16((ushort)Math.Min(ushort.MaxValue, player.MaxManapoints));
            OutputMessage.AddByte(player.GetSkillInfo(SkillType.Magic));
            OutputMessage.AddByte(player.GetSkillPercent(SkillType.Magic));
            
            OutputMessage.AddByte(player.SoulPoints);
            OutputMessage.AddUInt16(player.StaminaMinutes);
        }
    }
}
