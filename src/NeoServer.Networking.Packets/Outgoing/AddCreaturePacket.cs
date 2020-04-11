using System;
using System.Text;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class AddCreaturePacket : OutgoingPacket
    {
        private readonly ICreature creatureToAdd;
        private readonly IPlayer player;
        public AddCreaturePacket(IPlayer player, ICreature creatureToAdd)
        {
            this.creatureToAdd = creatureToAdd;
            this.player = player;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            var creatureId = creatureToAdd.CreatureId;

            var known = player.KnowsCreatureWithId(creatureId);
            if (known)
            {
                message.AddUInt16((ushort)GameOutgoingPacketType.AddKnownCreature);
                message.AddUInt32(creatureId);
            }
            else
            {
                message.AddUInt16((ushort)GameOutgoingPacketType.AddUnknownCreature);
                message.AddUInt32(player.ChooseToRemoveFromKnownSet()); //todo move to another class
                message.AddUInt32(creatureId);

                player.AddKnownCreature(creatureId); //todo: move to another class

                message.AddString(creatureToAdd.Name);
            }

            message.AddByte((byte)Math.Min(100, creatureToAdd.Hitpoints * 100 / creatureToAdd.MaxHitpoints));
            message.AddByte((byte)creatureToAdd.ClientSafeDirection);

            if (player.CanSee(creatureToAdd))
            {
                // Add creature outfit
                message.AddUInt16(creatureToAdd.Outfit.LookType);

                if (creatureToAdd.Outfit.LookType > 0)
                {
                    message.AddByte(creatureToAdd.Outfit.Head);
                    message.AddByte(creatureToAdd.Outfit.Body);
                    message.AddByte(creatureToAdd.Outfit.Legs);
                    message.AddByte(creatureToAdd.Outfit.Feet);
                    message.AddByte(creatureToAdd.Outfit.Addon);
                }
                else
                {
                    message.AddUInt16(creatureToAdd.Outfit.LookType);
                }
            }
            else
            {
                message.AddUInt16((ushort)0);
                message.AddUInt16((ushort)0);
            }

            message.AddByte(creatureToAdd.LightBrightness);
            message.AddByte(creatureToAdd.LightColor);

            message.AddUInt16(creatureToAdd.Speed);

            message.AddByte(creatureToAdd.Skull);
            message.AddByte(creatureToAdd.Shield);

            if (!known)
            {
                message.AddByte(0x00); //todo: guild emblem
            }

            message.AddByte(0x00); //todo see TFS
        }
    }
}
