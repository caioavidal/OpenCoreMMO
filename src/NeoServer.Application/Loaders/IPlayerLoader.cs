using NeoServer.Data.Entities;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Application.Loaders;

public interface IPlayerLoader
{
    IPlayer Load(PlayerEntity playerEntity);
    bool IsApplicable(PlayerEntity player);
}