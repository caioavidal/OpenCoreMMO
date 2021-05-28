using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Services
{
    public interface ISummonService
    {
        IMonster Summon(IMonster master, string summonName);
    }
}
