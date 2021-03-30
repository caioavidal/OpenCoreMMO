using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Common.Contracts.Creatures
{
    public interface IParty
    {
        IReadOnlyCollection<uint> Members { get; }
        bool IsEmpty { get; }

        event Action OnPartyOver;

        Result Invite(IPlayer by, IPlayer invitedPlayer);
        bool IsLeader(IPlayer player);
        bool JoinPlayer(IPlayer player);
        void RemoveMember(IPlayer player);
        void RevokeInvite(IPlayer by, IPlayer invitedPlayer);
    }
}
