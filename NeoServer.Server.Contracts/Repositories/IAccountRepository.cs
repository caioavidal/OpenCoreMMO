using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Server.Model;

namespace NeoServer.Server.Contracts.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> Get(string account, string password);
        void Create(Account account);
    }
}
