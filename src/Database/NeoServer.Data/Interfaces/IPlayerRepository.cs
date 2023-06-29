using System.Collections.Generic;
using System.Threading.Tasks;
using NeoServer.Data.Entities;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Data.Interfaces;

public interface IPlayerRepository : IBaseRepositoryNeo<PlayerEntity>
{
    Task UpdateAllPlayersToOffline();
    Task Add(PlayerEntity player);
    Task<List<PlayerOutfitAddonEntity>> GetOutfitAddons(int playerId);
    Task SaveBackpack(IPlayer player);

    /// <summary>
    ///     Updates player info data. (This method do not update inventory and items)
    /// </summary>
    /// <returns></returns>
    Task UpdatePlayer(IPlayer player);

    Task UpdatePlayers(IEnumerable<IPlayer> players);
    Task SavePlayerInventory(IPlayer player);
    Task UpdatePlayerOnlineStatus(uint playerId, bool status);
    Task<PlayerEntity> GetPlayer(string playerName);
}