using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Services;

public interface IPartyInviteService
{
    void Invite(IPlayer player, IPlayer invited);
}