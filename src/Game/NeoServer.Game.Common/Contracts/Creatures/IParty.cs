using NeoServer.Game.Contracts.Chats;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Common.Contracts.Creatures
{
    public interface IParty
    {
        IReadOnlyCollection<IPlayer> Members { get; }
        bool IsOver { get; }
        IPlayer Leader { get; }
        IReadOnlyCollection<uint> Invites { get; }
        IChatChannel Channel { get; }

        event Action OnPartyOver;

        Result ChangeLeadership(IPlayer from, IPlayer to);
        Result Invite(IPlayer by, IPlayer invitedPlayer);
        bool IsInvited(IPlayer player);
        bool IsLeader(IPlayer player);
        bool IsLeader(uint creatureId);
        bool IsMember(uint creatureId);
        bool IsMember(IPlayer player);
        bool JoinPlayer(IPlayer player);
        Result PassLeadership(IPlayer from);
        void RemoveMember(IPlayer player);
        void RevokeInvite(IPlayer by, IPlayer invitedPlayer);
    }
}
