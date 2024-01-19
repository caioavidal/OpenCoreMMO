using System;
using System.Collections.Generic;
using NeoServer.Application.Features.Chat;
using NeoServer.Application.Features.Chat.Channel;
using NeoServer.Application.Features.Chat.Channel.ExitChannel;
using NeoServer.Application.Features.Chat.Channel.ExitNpcChannel;
using NeoServer.Application.Features.Chat.Channel.JoinChannel;
using NeoServer.Application.Features.Chat.Channel.JoinPrivateChannel;
using NeoServer.Application.Features.Chat.Channel.OpenChannelList;
using NeoServer.Application.Features.Chat.PlayerSay;
using NeoServer.Application.Features.Chat.Vip;
using NeoServer.Application.Features.Chat.Vip.AddFriendToVip;
using NeoServer.Application.Features.Chat.Vip.RemoveFriendFromVip;
using NeoServer.Application.Features.Combat;
using NeoServer.Application.Features.Item.Container;
using NeoServer.Application.Features.Item.Container.CloseContainer;
using NeoServer.Application.Features.Item.Container.Navigate;
using NeoServer.Application.Features.Movement;
using NeoServer.Application.Features.Party;
using NeoServer.Application.Features.Party.EnableSharedExperience;
using NeoServer.Application.Features.Party.InviteToParty;
using NeoServer.Application.Features.Party.JoinParty;
using NeoServer.Application.Features.Party.LeaveParty;
using NeoServer.Application.Features.Party.PassPartyLeadership;
using NeoServer.Application.Features.Party.RevokeInvite;
using NeoServer.Application.Features.Player;
using NeoServer.Application.Features.Player.ChangeMode;
using NeoServer.Application.Features.Player.LookAt;
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
using NeoServer.Application.Features.Shop.CloseShop;
using NeoServer.Application.Features.Shop.Purchase;
using NeoServer.Application.Features.Shop.Sell;
using NeoServer.Application.Features.Trade;
using NeoServer.Application.Features.Trade.AcceptTrade;
using NeoServer.Application.Features.Trade.CancelTrade;
using NeoServer.Application.Features.Trade.RequestTrade;
using NeoServer.Networking.Handlers.Player;
using NeoServer.Server.Common.Contracts.Network.Enums;

namespace NeoServer.Networking.Handlers;

public static class InputHandlerMap
{
    public static readonly IReadOnlyDictionary<GameIncomingPacketType, Type> Data =
        new Dictionary<GameIncomingPacketType, Type>
        {
            [GameIncomingPacketType.PlayerLoginRequest] = typeof(AccountLoginPacketHandler),
            [GameIncomingPacketType.PlayerLogIn] = typeof(PlayerLogInPacketHandler),
            [GameIncomingPacketType.ChangeModes] = typeof(PlayerChangesModePacketHandler),
            [GameIncomingPacketType.PlayerLogOut] = typeof(PlayerLogOutPacketHandler),
            [GameIncomingPacketType.StopAllActions] = typeof(StopAllActionsPacketHandler),
            [GameIncomingPacketType.WalkEast] = typeof(PlayerMovePacketHandler),
            [GameIncomingPacketType.WalkWest] = typeof(PlayerMovePacketHandler),
            [GameIncomingPacketType.WalkSouth] = typeof(PlayerMovePacketHandler),
            [GameIncomingPacketType.WalkNorth] = typeof(PlayerMovePacketHandler),
            [GameIncomingPacketType.WalkNorteast] = typeof(PlayerMovePacketHandler),
            [GameIncomingPacketType.WalkNorthwest] = typeof(PlayerMovePacketHandler),
            [GameIncomingPacketType.WalkSoutheast] = typeof(PlayerMovePacketHandler),
            [GameIncomingPacketType.WalkSouthwest] = typeof(PlayerMovePacketHandler),
            [GameIncomingPacketType.TurnEast] = typeof(PlayerTurnPacketHandler),
            [GameIncomingPacketType.TurnWest] = typeof(PlayerTurnPacketHandler),
            [GameIncomingPacketType.TurnNorth] = typeof(PlayerTurnPacketHandler),
            [GameIncomingPacketType.TurnSouth] = typeof(PlayerTurnPacketHandler),
            [GameIncomingPacketType.AutoMove] = typeof(PlayerAutoWalkPacketHandler),
            [GameIncomingPacketType.Ping] = typeof(PingResponsePacketHandler),
            [GameIncomingPacketType.CancelAutoWalk] = typeof(PlayerCancelAutoWalkPacketHandler),
            [GameIncomingPacketType.ItemUse] = typeof(PlayerUseItemPacketHandler),
            [GameIncomingPacketType.ItemUseOn] = typeof(PlayerUseOnItemPacketHandler),
            [GameIncomingPacketType.ItemUseOnCreature] = typeof(PlayerUseItemOnCreaturePacketHandler),
            [GameIncomingPacketType.ContainerClose] = typeof(PlayerCloseContainerPacketHandler),
            [GameIncomingPacketType.ContainerUp] = typeof(PlayerGoBackContainerPacketHandler),
            [GameIncomingPacketType.ItemThrow] = typeof(PlayerMoveItemPacketPacketHandler),
            [GameIncomingPacketType.Attack] = typeof(PlayerAttackPacketHandler),
            [GameIncomingPacketType.LookAt] = typeof(PlayerLookAtPacketHandler),
            [GameIncomingPacketType.Speech] = typeof(PlayerSayPacketHandler),
            [GameIncomingPacketType.ChannelOpenPrivate] = typeof(PlayerOpenPrivateChannelPacketHandler),
            [GameIncomingPacketType.ChannelListRequest] = typeof(PlayerChannelListPacketHandler),
            [GameIncomingPacketType.ChannelOpen] = typeof(PlayerOpenChannelPacketHandler),
            [GameIncomingPacketType.ChannelClose] = typeof(PlayerCloseChannelPacketHandler),
            [GameIncomingPacketType.AddVip] = typeof(PlayerAddVipPacketHandler),
            [GameIncomingPacketType.RemoveVip] = typeof(PlayerRemoveVipPacketHandler),
            [GameIncomingPacketType.NpcChannelClose] = typeof(PlayerCloseNpcChannelPacketHandler),
            [GameIncomingPacketType.CloseShop] = typeof(PlayerCloseShopPacketHandler),
            [GameIncomingPacketType.PlayerSale] = typeof(PlayerSalePacketHandler),
            [GameIncomingPacketType.PlayerPurchase] = typeof(PlayerPurchasePacketHandler),
            [GameIncomingPacketType.PartyInvite] = typeof(PlayerInviteToPartyPacketHandler),
            [GameIncomingPacketType.PartyRevoke] = typeof(PlayerRevokeInvitePartyPacketHandler),
            [GameIncomingPacketType.PartyJoin] = typeof(PlayerJoinPartyPacketHandler),
            [GameIncomingPacketType.PartyLeave] = typeof(PlayerLeavePartyPacketHandler),
            [GameIncomingPacketType.PartyPassLeadership] = typeof(PlayerPassPartyLeadershipPacketHandler),
            [GameIncomingPacketType.EnableSharedExp] = typeof(PartyEnableSharedExperiencePacketHandler),
            [GameIncomingPacketType.WindowText] = typeof(PlayerWritePacketHandler),
            [GameIncomingPacketType.OutfitChangeRequest] = typeof(PlayerRequestOutfitPacketHandler),
            [GameIncomingPacketType.OutfitChangeCompleted] = typeof(PlayerOutfitChangePacketHandler),
            [GameIncomingPacketType.TradeRequest] = typeof(TradeRequestPacketHandler),
            [GameIncomingPacketType.TradeCancel] = typeof(TradeCancelPacketHandler),
            [GameIncomingPacketType.TradeAccept] = typeof(TradeAcceptPacketHandler)
        };
}