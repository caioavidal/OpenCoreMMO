using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Common.Contracts.Services
{
    public interface IPartyInviteService
    {
        void Invite(IPlayer player, IPlayer invited);
    }
}
