using NeoServer.Infrastructure.Data.Entities;

namespace NeoServer.Application.Loaders;

public interface IGuildLoader
{
    void Load(GuildEntity guildEntity);
}