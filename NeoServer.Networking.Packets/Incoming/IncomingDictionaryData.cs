using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeoServer.Networking.Packets.Incoming
{

    public class IncomingDictionaryData
    {
        public static readonly IReadOnlyDictionary<GameIncomingPacketType, Type> Data =
        new Dictionary<GameIncomingPacketType, Type>(){
            { GameIncomingPacketType.PlayerLoginRequest, typeof(AccountLoginPacket)}
        };

    }
}
