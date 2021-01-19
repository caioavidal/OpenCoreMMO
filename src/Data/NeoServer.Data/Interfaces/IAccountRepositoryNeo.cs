using NeoServer.Data.Model;
using System.Threading.Tasks;

namespace NeoServer.Data.Interfaces
{
    public interface IAccountRepositoryNeo : IBaseRepositoryNeo<AccountModel>
    {
        Task<AccountModel> GetById(int id);
        Task<AccountModel> GetByName(string name);
        Task<AccountModel> GetByEmail(string email);
        Task<AccountModel> Login(string name, string password);
    }
}
