using NeoServer.Data.Model;
using NeoServer.Game.Contracts;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeoServer.Server.Contracts.Repositories
{
    public interface IAccountRepository
    {
        Task<IAccountModel> Get(string account, string password);
        Task<AccountModel> FirstOrDefaultAsync(Expression<Func<AccountModel, bool>> filter);
        void Create(IAccountModel account);
    }
}
