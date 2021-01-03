using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class AddCreaturePacket : OutgoingPacket
    {
        private readonly IWalkableCreature creatureToAdd;
        private readonly IPlayer player;
        public AddCreaturePacket(IPlayer player, IWalkableCreature creatureToAdd)
        {
            this.creatureToAdd = creatureToAdd;
            this.player = player;
        }

        //todo: this code is duplicated?
        public override void WriteToMessage(INetworkMessage message) => message.AddBytes(creatureToAdd.GetRaw(player));

    }
}
