using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Data.Interfaces
{
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
        /// <param name="player"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        Task UpdatePlayer(IPlayer player, DbConnection conn = null);

        Task<PlayerModel> GetOnlinePlayer(string accountName);
        Task UpdatePlayerOnlineStatus(uint playerId, bool status);
    }
}