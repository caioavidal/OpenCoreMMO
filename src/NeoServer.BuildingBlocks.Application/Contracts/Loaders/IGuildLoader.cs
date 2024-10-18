using NeoServer.Data.Entities;

namespace NeoServer.BuildingBlocks.Application.Contracts.Loaders;

public interface IGuildLoader
{
    void Load(GuildEntity guildEntity);
}