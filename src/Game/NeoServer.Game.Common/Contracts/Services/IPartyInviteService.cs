using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Common.Contracts.Services
{
    public interface IPartyInviteService
    {
        void Invite(IPlayer player, IPlayer invited);
    }
}
