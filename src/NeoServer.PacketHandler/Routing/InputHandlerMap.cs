using NeoServer.Networking.Packets.Network.Enums;
using NeoServer.PacketHandler.Modules;
using NeoServer.PacketHandler.Modules.Chat.Channel.ExitChannel;
using NeoServer.PacketHandler.Modules.Chat.Channel.ExitNpcChannel;
using NeoServer.PacketHandler.Modules.Chat.Channel.JoinChannel;
using NeoServer.PacketHandler.Modules.Chat.Channel.JoinPrivateChannel;
using NeoServer.PacketHandler.Modules.Chat.Channel.OpenChannelList;
using NeoServer.PacketHandler.Modules.Chat.PlayerSay;
using NeoServer.PacketHandler.Modules.Chat.Vip.AddFriendToVip;
using NeoServer.PacketHandler.Modules.Chat.Vip.RemoveFriendFromVip;
using NeoServer.PacketHandler.Modules.Combat.AutoAttack;
using NeoServer.PacketHandler.Modules.ItemManagement.ContainerManagement.CloseContainer;
using NeoServer.PacketHandler.Modules.ItemManagement.ContainerManagement.Navigate;
using NeoServer.PacketHandler.Modules.Party.EnableSharedExperience;
using NeoServer.PacketHandler.Modules.Party.InviteToParty;
using NeoServer.PacketHandler.Modules.Party.JoinParty;
using NeoServer.PacketHandler.Modules.Party.LeaveParty;
using NeoServer.PacketHandler.Modules.Party.PassPartyLeadership;
using NeoServer.PacketHandler.Modules.Party.RevokeInvite;
using NeoServer.PacketHandler.Modules.Players.ChangeMode;
using NeoServer.PacketHandler.Modules.Players.LookAt;
using NeoServer.PacketHandler.Modules.Players.Outfit;
using NeoServer.PacketHandler.Modules.Players.StopAllActions;
using NeoServer.PacketHandler.Modules.Players.TurnTo;
using NeoServer.PacketHandler.Modules.Players.UseItem.UseItem;
using NeoServer.PacketHandler.Modules.Players.UseItem.UseOnCreature;
using NeoServer.PacketHandler.Modules.Players.UseItem.UseOnItem;
using NeoServer.PacketHandler.Modules.Players.Walk;
using NeoServer.PacketHandler.Modules.Players.Write;
using NeoServer.PacketHandler.Modules.Session.ListCharacters;
using NeoServer.PacketHandler.Modules.Session.LogIn;
using NeoServer.PacketHandler.Modules.Session.LogOut;
using NeoServer.PacketHandler.Modules.Session.Ping;
using NeoServer.PacketHandler.Modules.Shopping.CloseShop;
using NeoServer.PacketHandler.Modules.Shopping.Purchase;
using NeoServer.PacketHandler.Modules.Shopping.Sell;
using NeoServer.PacketHandler.Modules.Trading.AcceptTrade;
using NeoServer.PacketHandler.Modules.Trading.CancelTrade;
using NeoServer.PacketHandler.Modules.Trading.RequestTrade;

namespace NeoServer.PacketHandler.Routing;

public static class InputHandlerMap
{
    public static readonly IReadOnlyDictionary<GameIncomingPacketType, Type> Data =
        new Dictionary<GameIncomingPacketType, Type>
        {
            [GameIncomingPacketType.PlayerLoginRequest] = typeof(AccountLoginPacketHandler),
            [GameIncomingPacketType.PlayerLogIn] = typeof(PlayerLogInPacketHandler),
            [GameIncomingPacketType.PlayerLogOut] = typeof(PlayerLogOutPacketHandler),
            [GameIncomingPacketType.Ping] = typeof(PingResponsePacketHandler), 
            [GameIncomingPacketType.ChangeModes] = typeof(PlayerChangesModePacketHandler),
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
            [GameIncomingPacketType.CancelAutoWalk] = typeof(PlayerCancelAutoWalkPacketHandler),
            [GameIncomingPacketType.ItemUse] = typeof(PlayerUseItemPacketHandler),
            [GameIncomingPacketType.ItemUseOn] = typeof(PlayerUseOnItemPacketHandler),
            [GameIncomingPacketType.ItemUseOnCreature] = typeof(PlayerUseItemOnCreaturePacketHandler),
            [GameIncomingPacketType.ItemThrow] = typeof(PlayerMoveItemPacketPacketHandler),
            [GameIncomingPacketType.LookAt] = typeof(PlayerLookAtPacketHandler),
            [GameIncomingPacketType.WindowText] = typeof(PlayerWritePacketHandler),
            [GameIncomingPacketType.OutfitChangeRequest] = typeof(PlayerRequestOutfitPacketHandler),
            [GameIncomingPacketType.OutfitChangeCompleted] = typeof(PlayerOutfitChangePacketHandler),
            [GameIncomingPacketType.ContainerClose] = typeof(PlayerCloseContainerPacketHandler),
            [GameIncomingPacketType.ContainerUp] = typeof(PlayerGoBackContainerPacketHandler),
            [GameIncomingPacketType.Attack] = typeof(PlayerAttackPacketHandler),
            [GameIncomingPacketType.Speech] = typeof(PlayerSayPacketHandler),
            [GameIncomingPacketType.ChannelOpenPrivate] = typeof(PlayerOpenPrivateChannelPacketHandler),
            [GameIncomingPacketType.ChannelListRequest] = typeof(PlayerChannelListPacketHandler),
            [GameIncomingPacketType.ChannelOpen] = typeof(PlayerJoinChannelPacketHandler),
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
            [GameIncomingPacketType.TradeRequest] = typeof(TradeRequestPacketHandler),
            [GameIncomingPacketType.TradeCancel] = typeof(TradeCancelPacketHandler),
            [GameIncomingPacketType.TradeAccept] = typeof(TradeAcceptPacketHandler)
        };
}