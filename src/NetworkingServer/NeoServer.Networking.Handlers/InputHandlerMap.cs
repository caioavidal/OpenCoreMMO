using System;
using System.Collections.Generic;
using NeoServer.Application.Features.Chat;
using NeoServer.Application.Features.Chat.Channel;
using NeoServer.Application.Features.Chat.Channel.PacketHandlers;
using NeoServer.Application.Features.Chat.PlayerSay;
using NeoServer.Application.Features.Chat.Vip;
using NeoServer.Application.Features.Combat;
using NeoServer.Application.Features.Item.Container;
using NeoServer.Application.Features.Movement;
using NeoServer.Application.Features.Party;
using NeoServer.Application.Features.Player;
using NeoServer.Application.Features.Player.Movement;
using NeoServer.Application.Features.Player.Outfit;
using NeoServer.Application.Features.Player.Ping;
using NeoServer.Application.Features.Player.UseItem.UseItem;
using NeoServer.Application.Features.Player.UseItem.UseOnCreature;
using NeoServer.Application.Features.Player.UseItem.UseOnItem;
using NeoServer.Application.Features.Player.Walk;
using NeoServer.Application.Features.Session.LogIn;
using NeoServer.Application.Features.Session.LogIn.Account;
using NeoServer.Application.Features.Session.LogOut;
using NeoServer.Application.Features.Shop;
using NeoServer.Application.Features.Trade;
using NeoServer.Application.Features.Trade.TradeRequest;
using NeoServer.Networking.Handlers.Player;
using NeoServer.Server.Common.Contracts.Network.Enums;

namespace NeoServer.Networking.Handlers;

public static class InputHandlerMap
{
    public static readonly IReadOnlyDictionary<GameIncomingPacketType, Type> Data =
        new Dictionary<GameIncomingPacketType, Type>
        {
            [GameIncomingPacketType.PlayerLoginRequest] = typeof(AccountLoginHandler),
            [GameIncomingPacketType.PlayerLogIn] = typeof(PlayerLogInHandler),
            [GameIncomingPacketType.ChangeModes] = typeof(PlayerChangesModeHandler),
            [GameIncomingPacketType.PlayerLogOut] = typeof(PlayerLogOutHandler),
            [GameIncomingPacketType.StopAllActions] = typeof(StopAllActionsHandler),
            [GameIncomingPacketType.WalkEast] = typeof(PlayerMoveHandler),
            [GameIncomingPacketType.WalkWest] = typeof(PlayerMoveHandler),
            [GameIncomingPacketType.WalkSouth] = typeof(PlayerMoveHandler),
            [GameIncomingPacketType.WalkNorth] = typeof(PlayerMoveHandler),
            [GameIncomingPacketType.WalkNorteast] = typeof(PlayerMoveHandler),
            [GameIncomingPacketType.WalkNorthwest] = typeof(PlayerMoveHandler),
            [GameIncomingPacketType.WalkSoutheast] = typeof(PlayerMoveHandler),
            [GameIncomingPacketType.WalkSouthwest] = typeof(PlayerMoveHandler),
            [GameIncomingPacketType.TurnEast] = typeof(PlayerTurnHandler),
            [GameIncomingPacketType.TurnWest] = typeof(PlayerTurnHandler),
            [GameIncomingPacketType.TurnNorth] = typeof(PlayerTurnHandler),
            [GameIncomingPacketType.TurnSouth] = typeof(PlayerTurnHandler),
            [GameIncomingPacketType.AutoMove] = typeof(PlayerAutoWalkHandler),
            [GameIncomingPacketType.Ping] = typeof(PingResponseHandler),
            [GameIncomingPacketType.CancelAutoWalk] = typeof(PlayerCancelAutoWalkHandler),
            [GameIncomingPacketType.ItemUse] = typeof(PlayerUseItemHandler),
            [GameIncomingPacketType.ItemUseOn] = typeof(PlayerUseOnItemHandler),
            [GameIncomingPacketType.ItemUseOnCreature] = typeof(PlayerUseItemOnCreatureHandler),
            [GameIncomingPacketType.ContainerClose] = typeof(PlayerCloseContainerHandler),
            [GameIncomingPacketType.ContainerUp] = typeof(PlayerGoBackContainerHandler),
            [GameIncomingPacketType.ItemThrow] = typeof(PlayerMoveItemHandler),
            [GameIncomingPacketType.Attack] = typeof(PlayerAttackHandler),
            [GameIncomingPacketType.LookAt] = typeof(PlayerLookAtHandler),
            [GameIncomingPacketType.Speech] = typeof(PlayerSayHandler),
            [GameIncomingPacketType.ChannelOpenPrivate] = typeof(PlayerOpenPrivateChannelPacketHandler),
            [GameIncomingPacketType.ChannelListRequest] = typeof(PlayerChannelListPacketHandler),
            [GameIncomingPacketType.ChannelOpen] = typeof(PlayerOpenChannelPacketHandler),
            [GameIncomingPacketType.ChannelClose] = typeof(PlayerCloseChannelPacketHandler),
            [GameIncomingPacketType.AddVip] = typeof(PlayerAddVipPacketHandler),
            [GameIncomingPacketType.RemoveVip] = typeof(PlayerRemoveVipPacketHandler),
            [GameIncomingPacketType.NpcChannelClose] = typeof(PlayerCloseNpcChannelPacketHandler),
            [GameIncomingPacketType.CloseShop] = typeof(PlayerCloseShopHandler),
            [GameIncomingPacketType.PlayerSale] = typeof(PlayerSaleHandler),
            [GameIncomingPacketType.PlayerPurchase] = typeof(PlayerPurchaseHandler),
            [GameIncomingPacketType.PartyInvite] = typeof(PlayerInviteToPartyHandler),
            [GameIncomingPacketType.PartyRevoke] = typeof(PlayerRevokeInvitePartyHandler),
            [GameIncomingPacketType.PartyJoin] = typeof(PlayerJoinPartyHandler),
            [GameIncomingPacketType.PartyLeave] = typeof(PlayerLeavePartyHandler),
            [GameIncomingPacketType.PartyPassLeadership] = typeof(PlayerPassPartyLeadershipHandler),
            [GameIncomingPacketType.EnableSharedExp] = typeof(PartyEnableSharedExperienceHandler),
            [GameIncomingPacketType.WindowText] = typeof(PlayerWriteEventHandler),
            [GameIncomingPacketType.OutfitChangeRequest] = typeof(PlayerRequestOutfitHandler),
            [GameIncomingPacketType.OutfitChangeCompleted] = typeof(PlayerOutfitChangeHandler),
            [GameIncomingPacketType.TradeRequest] = typeof(TradeRequestHandler),
            [GameIncomingPacketType.TradeCancel] = typeof(TradeCancelHandler),
            [GameIncomingPacketType.TradeAccept] = typeof(TradeAcceptHandler)
        };
}