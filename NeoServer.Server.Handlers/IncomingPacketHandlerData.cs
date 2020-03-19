using NeoServer.Server.Handlers.Authentication;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace NeoServer.Networking.Packets.Incoming
{

    public class IncomingPacketHandlerData
    {
        public static readonly IReadOnlyDictionary<GameIncomingPacketType, Type> Data =
        new Dictionary<GameIncomingPacketType, Type>(){
            { GameIncomingPacketType.PlayerLoginRequest, typeof(AccountLoginEventHandler)},
            { GameIncomingPacketType.PlayerLogIn, typeof(PlayerLogInEventHandler)}
        };
    }
}
