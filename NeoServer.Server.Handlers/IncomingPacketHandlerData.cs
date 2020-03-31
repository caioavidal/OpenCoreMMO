using NeoServer.Server.Contracts.Network.Enums;
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
            { GameIncomingPacketType.PlayerLoginRequest, typeof(AccountLoginHandler)},
            { GameIncomingPacketType.PlayerLogIn, typeof(PlayerLogInHandler)},
            { GameIncomingPacketType.ChangeModes, typeof(PlayerChangesModeHandler)},
            { GameIncomingPacketType.PlayerLogOut, typeof(PlayerLogOutHandler)},
            { GameIncomingPacketType.WalkEast, typeof(PlayerMoveHandler)},
            { GameIncomingPacketType.WalkWest, typeof(PlayerMoveHandler)},
            { GameIncomingPacketType.WalkSouth, typeof(PlayerMoveHandler)},
            { GameIncomingPacketType.WalkNorth, typeof(PlayerMoveHandler)},
            { GameIncomingPacketType.WalkNorteast, typeof(PlayerMoveHandler)},
            { GameIncomingPacketType.WalkNorthwest, typeof(PlayerMoveHandler)},
            { GameIncomingPacketType.WalkSoutheast, typeof(PlayerMoveHandler)},
            { GameIncomingPacketType.WalkSouthwest, typeof(PlayerMoveHandler)}
        };
    }
}
