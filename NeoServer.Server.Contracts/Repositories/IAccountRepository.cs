using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Server.Model;

namespace NeoServer.Server.Contracts.Repositories
{
    public interface IAccountRepository
    {
        Task<AccountModel> Get(string account, string password);
        Task<AccountModel> FirstOrDefaultAsync(Expression<Func<AccountModel, bool>> filter);
        void Create(AccountModel account);
    }
}
