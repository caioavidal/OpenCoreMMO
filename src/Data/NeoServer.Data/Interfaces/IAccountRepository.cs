using NeoServer.Data.Model;
using NeoServer.Server.Model.Players;
using System.Linq;
using System.Threading.Tasks;

namespace NeoServer.Data.Interfaces
{
    public interface IAccountRepository : IBaseRepositoryNeo<AccountModel>
    {
        Task<AccountModel> GetById(int id);
        Task<AccountModel> GetByName(string name);
        Task<AccountModel> GetByEmail(string email);
        IQueryable<AccountModel> GetAccount(string name, string password);
        Task<PlayerModel> GetPlayer(string name);
        Task AddPlayerToVipList(int accountId, int playerId);
        Task RemoveFromVipList(int accountId, int playerId);
        IQueryable<PlayerModel> GetPlayer(string accountName, string password, string charName);
    }
}
