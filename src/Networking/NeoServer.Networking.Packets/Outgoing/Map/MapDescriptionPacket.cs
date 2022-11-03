using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Map;

public class MapDescriptionPacket : OutgoingPacket
{
    private readonly IMap map;
    private readonly IPlayer player;

    public MapDescriptionPacket(IPlayer player, IMap map)
    {
        this.player = player;
        this.map = map;
    }

    public override void WriteToMessage(INetworkMessage message)
    {
        message.AddByte((int)GameOutgoingPacketType.MapDescription);
        message.AddLocation(player.Location);

        message.AddBytes(GetMapDescrition(player, map));
    }

    private byte[] GetMapDescrition(IPlayer player, IMap map)
    {
        var location = player.Location;
        return map.GetDescription(player, (ushort)(location.X - 8), (ushort)(location.Y - 6), location.Z).ToArray();
    }
}