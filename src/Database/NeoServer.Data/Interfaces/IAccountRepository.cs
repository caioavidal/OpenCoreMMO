using System.Threading.Tasks;
using NeoServer.Data.Model;

namespace NeoServer.Data.Interfaces;

public interface IAccountRepository : IBaseRepositoryNeo<AccountModel>
{
    Task<AccountModel> GetAccount(string name, string password);
    Task AddPlayerToVipList(int accountId, int playerId);
    Task RemoveFromVipList(int accountId, int playerId);
    Task<PlayerModel> GetPlayer(string accountName, string password, string charName);
    Task<PlayerModel> GetOnlinePlayer(string accountName);
    Task<int> Ban(uint accountId, string reason, uint bannedByAccountId);
}