using NeoServer.Data.Entities;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Application.Common.Contracts.Loaders;

public interface IPlayerLoader
{
    IPlayer Load(PlayerEntity playerEntity);
    bool IsApplicable(PlayerEntity player);
}