using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Player;

public class PlayerOutFitWindowPacket : OutgoingPacket
{
    private readonly IPlayer player;
    private readonly IEnumerable<IPlayerOutFit> _playerOutFits;

    public PlayerOutFitWindowPacket(IPlayer player, IEnumerable<IPlayerOutFit> playerOutFits)
    {
        this.player = player;
        _playerOutFits = playerOutFits;
    }

    public override void WriteToMessage(INetworkMessage message)
    {
        message.AddByte((byte)GameOutgoingPacketType.OutfitWindow);
        message.AddUInt16(player.Outfit.LookType);

        if (player.Outfit.LookType != 0)
        {
            message.AddByte(player.Outfit.Head);
            message.AddByte(player.Outfit.Body);
            message.AddByte(player.Outfit.Legs);
            message.AddByte(player.Outfit.Feet);
            message.AddByte(player.Outfit.Addon);
        }
        
        message.AddByte((byte)_playerOutFits.Count());
        foreach (var playerOutFit in _playerOutFits)
        {
            message.AddUInt16(playerOutFit.LookType); 
            message.AddString(playerOutFit.Name);
            message.AddByte(3); // Enable fully Addon to outfit.
        }
    
    }
}