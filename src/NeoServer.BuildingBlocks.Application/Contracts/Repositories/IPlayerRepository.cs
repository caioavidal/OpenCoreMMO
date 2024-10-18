using NeoServer.Data.Entities;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.BuildingBlocks.Application.Contracts.Repositories;

public interface IPlayerRepository : IBaseRepositoryNeo<PlayerEntity>
{
    Task UpdateAllPlayersToOffline();
    Task Add(PlayerEntity player);
    Task<List<PlayerOutfitAddonEntity>> GetOutfitAddons(int playerId);
    Task UpdatePlayers(IEnumerable<IPlayer> players);
    Task UpdatePlayerOnlineStatus(uint playerId, bool status);
    Task<PlayerEntity> GetPlayer(string playerName);

    /// <summary>
    ///     Save player info, inventory, backpack and depot
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    Task SavePlayer(IPlayer player);
}