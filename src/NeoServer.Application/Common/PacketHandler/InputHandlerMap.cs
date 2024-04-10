using NeoServer.Application.Features.Chat.Channel.ExitChannel;
using NeoServer.Application.Features.Chat.Channel.ExitNpcChannel;
using NeoServer.Application.Features.Chat.Channel.JoinChannel;
using NeoServer.Application.Features.Chat.Channel.JoinPrivateChannel;
using NeoServer.Application.Features.Chat.Channel.OpenChannelList;
using NeoServer.Application.Features.Chat.PlayerSay;
using NeoServer.Application.Features.Chat.Vip.AddFriendToVip;
using NeoServer.Application.Features.Chat.Vip.RemoveFriendFromVip;
using NeoServer.Application.Features.Combat.AutoAttack;
using NeoServer.Application.Features.Item.Container.CloseContainer;
using NeoServer.Application.Features.Item.Container.Navigate;
using NeoServer.Application.Features.Movement;
using NeoServer.Application.Features.Party.EnableSharedExperience;
using NeoServer.Application.Features.Party.InviteToParty;
using NeoServer.Application.Features.Party.JoinParty;
using NeoServer.Application.Features.Party.LeaveParty;
using NeoServer.Application.Features.Party.PassPartyLeadership;
using NeoServer.Application.Features.Party.RevokeInvite;
using NeoServer.Application.Features.Player.ChangeMode;
using NeoServer.Application.Features.Player.LookAt;
using NeoServer.Application.Features.Player.Outfit;
using NeoServer.Application.Features.Player.StopAllActions;
using NeoServer.Application.Features.Player.TurnTo;
using NeoServer.Application.Features.Player.UseItem.UseItem;
using NeoServer.Application.Features.Player.UseItem.UseOnCreature;
using NeoServer.Application.Features.Player.UseItem.UseOnItem;
using NeoServer.Application.Features.Player.Walk;
using NeoServer.Application.Features.Player.Write;
using NeoServer.Application.Features.Session.ListCharacters;
using NeoServer.Application.Features.Session.LogIn;
using NeoServer.Application.Features.Session.LogOut;
using NeoServer.Application.Features.Session.Ping;
using NeoServer.Application.Features.Shop.CloseShop;
using NeoServer.Application.Features.Shop.Purchase;
using NeoServer.Application.Features.Shop.Sell;
using NeoServer.Application.Features.Trade.AcceptTrade;
using NeoServer.Application.Features.Trade.CancelTrade;
using NeoServer.Application.Features.Trade.RequestTrade;
using NeoServer.Networking.Packets.Network.Enums;

namespace NeoServer.Application.Common.PacketHandler;

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
            [GameIncomingPacketType.WalkEast] = typeof(PlayerWalkPacketHandler),
            [GameIncomingPacketType.WalkWest] = typeof(PlayerWalkPacketHandler),
            [GameIncomingPacketType.WalkSouth] = typeof(PlayerWalkPacketHandler),
            [GameIncomingPacketType.WalkNorth] = typeof(PlayerWalkPacketHandler),
            [GameIncomingPacketType.WalkNorteast] = typeof(PlayerWalkPacketHandler),
            [GameIncomingPacketType.WalkNorthwest] = typeof(PlayerWalkPacketHandler),
            [GameIncomingPacketType.WalkSoutheast] = typeof(PlayerWalkPacketHandler),
            [GameIncomingPacketType.WalkSouthwest] = typeof(PlayerWalkPacketHandler),
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