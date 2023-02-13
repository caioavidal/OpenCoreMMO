using System.Collections.Generic;
using System.Threading.Tasks;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Data.Interfaces;

public interface IAccountRepository : IBaseRepositoryNeo<AccountModel>
{
    Task<AccountModel> GetAccount(string name, string password);
    Task<PlayerModel> GetPlayer(string name);
    Task AddPlayerToVipList(int accountId, int playerId);
    Task RemoveFromVipList(int accountId, int playerId);
    Task<PlayerModel> GetPlayer(string accountName, string password, string charName);
    Task UpdatePlayers(IEnumerable<IPlayer> players);

    /// <summary>
    ///     Updates player info data. (This method do not update inventory and items)
    /// </summary>
    /// <returns></returns>
    Task UpdatePlayer(IPlayer player);

    Task<PlayerModel> GetOnlinePlayer(string accountName);
    Task UpdatePlayerOnlineStatus(uint playerId, bool status);

    Task SavePlayerInventory(IPlayer player);
}