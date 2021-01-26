using NeoServer.Data.Model;
using NeoServer.Server.Model.Players;
using System.Threading.Tasks;

namespace NeoServer.Data.Interfaces
{
    public interface IAccountRepository : IBaseRepositoryNeo<AccountModel>
    {
        Task<AccountModel> GetById(int id);
        Task<AccountModel> GetByName(string name);
        Task<AccountModel> GetByEmail(string email);
        Task<AccountModel> Login(string name, string password);
        Task<PlayerModel> GetPlayer(string name);
    }
}
