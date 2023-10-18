using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Chat;

public interface IUserChat
{
    bool Removed { get; }
    bool IsMuted { get; }
    IPlayer Player { get; init; }
}