using NeoServer.Server.Handlers.Authentication;
using NeoServer.Server.Handlers.Players;
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
            { GameIncomingPacketType.PlayerLogIn, typeof(PlayerLogInEventHandler)},
            { GameIncomingPacketType.ChangeModes, typeof(PlayerChangesModeEventHandler)},
            { GameIncomingPacketType.PlayerLogOut, typeof(PlayerLogOutEventHandler)},
            { GameIncomingPacketType.WalkEast, typeof(PlayerMoveEventHandler)},
            { GameIncomingPacketType.WalkWest, typeof(PlayerMoveEventHandler)},
            { GameIncomingPacketType.WalkSouth, typeof(PlayerMoveEventHandler)},
            { GameIncomingPacketType.WalkNorth, typeof(PlayerMoveEventHandler)},
            { GameIncomingPacketType.WalkNorteast, typeof(PlayerMoveEventHandler)},
            { GameIncomingPacketType.WalkNorthwest, typeof(PlayerMoveEventHandler)},
            { GameIncomingPacketType.WalkSoutheast, typeof(PlayerMoveEventHandler)},
            { GameIncomingPacketType.WalkSouthwest, typeof(PlayerMoveEventHandler)}
        };
    }
}
