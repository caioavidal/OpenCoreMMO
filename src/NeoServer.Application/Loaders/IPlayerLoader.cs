using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Infrastructure.Data.Entities;

namespace NeoServer.Application.Loaders;

public interface IPlayerLoader
{
    IPlayer Load(PlayerEntity playerEntity);
    bool IsApplicable(PlayerEntity player);
}