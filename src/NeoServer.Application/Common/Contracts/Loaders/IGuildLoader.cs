using NeoServer.Data.Entities;

namespace NeoServer.Application.Common.Contracts.Loaders;

public interface IGuildLoader
{
    void Load(GuildEntity guildEntity);
}