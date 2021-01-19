using NeoServer.Server.Contracts.Network.Enums;
using NeoServer.Server.Handlers.Authentication;
using NeoServer.Server.Handlers.Player;
using NeoServer.Server.Handlers.Players;
using System;
using System.Collections.Generic;

namespace NeoServer.Networking.Packets.Incoming
{

    public sealed class InputHandlerMap
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
            { GameIncomingPacketType.WalkSouthwest, typeof(PlayerMoveHandler)},
            { GameIncomingPacketType.TurnEast, typeof(PlayerTurnHandler)},
            { GameIncomingPacketType.TurnWest, typeof(PlayerTurnHandler)},
            { GameIncomingPacketType.TurnNorth, typeof(PlayerTurnHandler)},
            { GameIncomingPacketType.TurnSouth, typeof(PlayerTurnHandler)},
            { GameIncomingPacketType.AutoMove, typeof(PlayerAutoWalkHandler)},
            { GameIncomingPacketType.Ping, typeof(PlayerPingResponseHandler)},
            { GameIncomingPacketType.CancelAutoWalk, typeof(PlayerCancelAutoWalkHandler)},
            { GameIncomingPacketType.ItemUse, typeof(PlayerUseItemHandler)},
            { GameIncomingPacketType.ItemUseOn, typeof(PlayerUseOnItemHandler)},
            { GameIncomingPacketType.ItemUseOnCreature, typeof(PlayerUseOnCreatureHandler)},
            { GameIncomingPacketType.ContainerClose, typeof(PlayerCloseContainerHandler)},
            { GameIncomingPacketType.ContainerUp, typeof(PlayerGoBackContainerHandler) },
            { GameIncomingPacketType.ItemThrow, typeof(PlayerThrowItemHandler)},
            { GameIncomingPacketType.Attack, typeof(PlayerAttackHandler)},
            { GameIncomingPacketType.LookAt, typeof(PlayerLookAtHandler)},
            { GameIncomingPacketType.Speech, typeof(PlayerSayHandler)},
            { GameIncomingPacketType.ChannelOpenPrivate, typeof(PlayerOpenPrivateChannelHandler)}
        };
    }
}
