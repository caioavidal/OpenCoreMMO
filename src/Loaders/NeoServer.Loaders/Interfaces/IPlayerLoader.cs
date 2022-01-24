using NeoServer.Data.Model;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Loaders.Interfaces;

public interface IPlayerLoader
{
    IPlayer Load(PlayerModel player);
    bool IsApplicable(PlayerModel player);
}