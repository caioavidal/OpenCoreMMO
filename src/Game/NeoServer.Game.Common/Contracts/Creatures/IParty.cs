using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Contracts.Creatures
{
    public interface IParty
    {
        IReadOnlyCollection<uint> Members { get; }

        event RevokePartyInvite OnRevokePartyInvite;

        Result Invite(IPlayer by, IPlayer invitedPlayer);
        bool IsLeader(IPlayer player);
        bool JoinPlayer(IPlayer player);
        void RemoveMember(IPlayer player);
        void RevokeInvite(IPlayer by, IPlayer invitedPlayer);
    }
}
